﻿using AbyssCLI.ABI;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using UnityEngine;

namespace AbyssEngine
{
    internal class Host
    {
        public readonly bool IsValid;
        public readonly AbyssCLI.ABI.UIActionWriter CallFunc; //all protobuf message sender
        private readonly System.Diagnostics.Process _host_proc;
        private readonly ConcurrentQueue<RenderAction> _render_action_queue;
        private readonly ConcurrentQueue<Exception> _error_queue;
        private readonly Thread _reader_th;
        private readonly Thread _error_reader_th;
        public Host(string root_key_path)
        {
            byte[] root_key;
            try
            {
                root_key = System.IO.File.ReadAllBytes(root_key_path);
            }
            catch
            {
                IsValid = false;
                return;
            }
            //run host process with pipe
            _host_proc = new System.Diagnostics.Process();
            _host_proc.StartInfo.FileName = ".\\AbyssCLI\\AbyssCLI.exe";
            _host_proc.StartInfo.UseShellExecute = false;
            _host_proc.StartInfo.CreateNoWindow = true;
            _host_proc.StartInfo.RedirectStandardInput = true;
            _host_proc.StartInfo.RedirectStandardOutput = true;
            _host_proc.StartInfo.RedirectStandardError = true;
            _host_proc.Start();

            CallFunc = new AbyssCLI.ABI.UIActionWriter(
                _host_proc.StandardInput.BaseStream
            )
            {
                AutoFlush = true
            };
            _render_action_queue = new();
            _error_queue = new();

            _reader_th = new Thread(() =>
            {
                try
                {
                    var reader = new BinaryReader(_host_proc.StandardOutput.BaseStream);
                    while (true)
                    {
                        int length = reader.ReadInt32();
                        if (length <= 0)
                        {
                            throw new Exception("invalid length message");
                        }
                        byte[] data = reader.ReadBytes(length);
                        if (data.Length != length)
                        {
                            throw new Exception("invalid length message");
                        }

                        var message = RenderAction.Parser.ParseFrom(data);
                        _render_action_queue.Enqueue(message);
                    }
                }
                catch (Exception e)
                {
                    if (e is not System.IO.EndOfStreamException)
                        _error_queue.Enqueue(e);
                }
            });
            _reader_th.Start();

            _error_reader_th = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        var line = _host_proc.StandardError.ReadLine();
                        if (line == null)
                            return; //cerr closed.

                        _error_queue.Enqueue(new Exception(line));
                    }
                }
                catch (Exception e)
                {
                    _error_queue.Enqueue(e);
                }
            });
            _error_reader_th.Start();

            //initialize host
            CallFunc.Init(Google.Protobuf.ByteString.CopyFrom(root_key), Path.GetFileNameWithoutExtension(root_key_path));

            //#if UNITY_EDITOR
            //            CallFunc.Init(Google.Protobuf.ByteString.CopyFrom(root_key), "editor");
            //#else
            //            CallFunc.Init(Google.Protobuf.ByteString.CopyFrom(root_key), "build");
            //#endif
            IsValid = true;
        }
        public void Close()
        {
            if (!_host_proc.HasExited)
            {
                _host_proc.Kill();
                _host_proc.WaitForExit();
            }

            _reader_th.Join();
            _error_reader_th.Join();
        }
        public bool TryPopRenderAction(out RenderAction msg)
        {
            return _render_action_queue.TryDequeue(out msg);
        }
        public int GetLeftoverRenderActionCount()
        {
            return _render_action_queue.Count;
        }
        public bool TryPopException(out Exception e)
        {
            return _error_queue.TryDequeue(out e);
        }
    }
}
