using AbyssCLI.ABI;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Executor
{
    private Dictionary<int, GameObject> _game_objects;
    private Dictionary<int, AbyssEngine.Component.IComponent> _components;
    private void CreateElement(RenderAction.Types.CreateElement args)
    {
        GameObject newGO = new(args.ElementId.ToString());
        newGO.transform.SetParent(_game_objects[args.ParentId].transform, false);
        _game_objects[args.ElementId] = newGO;
    }
    private void MoveElement(RenderAction.Types.MoveElement args)
    {
        _game_objects[args.ElementId].transform.SetParent(_game_objects[args.NewParentId].transform, true);
    }
    private void DeleteElement(RenderAction.Types.DeleteElement args)
    {
        GameObject.Destroy(_game_objects[args.ElementId]);
        _game_objects.Remove(args.ElementId);
    }
    private void ElemSetPos(RenderAction.Types.ElemSetPos args)
    {
        _game_objects[args.ElementId].transform.SetLocalPositionAndRotation(new Vector3(args.Pos.X, args.Pos.Y, args.Pos.Z), new Quaternion(args.Rot.X, args.Rot.Y, args.Rot.Z, args.Rot.W));
    }
    private void CreateItem(RenderAction.Types.CreateItem args)
    {
        GameObject newGO = new("I" + args.ElementId.ToString());
        newGO.transform.SetParent(_game_objects[0].transform, false);
        _game_objects[args.ElementId] = newGO;

        if (args.SharerHash == _local_hash)
            uiHandler.LocalItemSection.CreateItem(this, args.ElementId, new(args.Uuid.ToByteArray()));
        else
            uiHandler.MemberItemSection.CreateItem(args.SharerHash, args.ElementId);
    }
    private void DeleteItem(RenderAction.Types.DeleteItem args)
    {
        GameObject.Destroy(_game_objects[args.ElementId]);
        _game_objects.Remove(args.ElementId);

        if (uiHandler.MemberItemSection.IsMemberItem(args.ElementId))
            uiHandler.MemberItemSection.RemoveItem(args.ElementId);
        else
            uiHandler.LocalItemSection.RemoveItem(args.ElementId);
    }
    private void ItemSetIcon(RenderAction.Types.ItemSetIcon args)
    {
        var component = (AbyssEngine.Component.Image)(_components[args.ImageId]);
        var icon = component.UnityTexture2D;

        if (uiHandler.MemberItemSection.IsMemberItem(args.ElementId))
            uiHandler.MemberItemSection.UpdateIcon(args.ElementId, icon);
        else
            uiHandler.LocalItemSection.UpdateIcon(args.ElementId, icon);
    }
    private void MemberInfo(RenderAction.Types.MemberInfo args)
    {
        uiHandler.MemberProfileSection.CreateProfile(args.PeerHash);
        uiHandler.MemberItemSection.CreateMember(args.PeerHash);
    }
    private void MemberLeave(RenderAction.Types.MemberLeave args)
    {
        uiHandler.MemberItemSection.RemoveMember(args.PeerHash);
        uiHandler.MemberProfileSection.RemoveProfile(args.PeerHash);
    }
    private void CreateImage(RenderAction.Types.CreateImage args)
    {
        _components[args.ImageId] = new AbyssEngine.Component.Image(args.File);
    }
    private void DeleteImage(RenderAction.Types.DeleteImage args)
    {
        DeleteComponent(args.ImageId);
    }
    private void CreateMaterialV(RenderAction.Types.CreateMaterialV args)
    {
        _components[args.MaterialId] = new AbyssEngine.Component.Material(
            commonShaderLoader.Get(args.ShaderName),
            commonShaderLoader.GetParameterIDMap(args.ShaderName)
        );
    }
    private void CreateMaterialF(RenderAction.Types.CreateMaterialF args)
    {
        throw new NotImplementedException();
    }
    private void MaterialSetParamV(RenderAction.Types.MaterialSetParamV args)
    {
        var mat = _components[args.MaterialId] as AbyssEngine.Component.Material;
        switch (args.Param.ValCase)
        {
            case AnyVal.ValOneofCase.Int:
                mat.UnityMaterial.SetInteger(args.ParamName, args.Param.Int);
                break;
            case AnyVal.ValOneofCase.Double:
                mat.UnityMaterial.SetFloat(args.ParamName, (float)args.Param.Double);
                break;
            case AnyVal.ValOneofCase.Vec2:
                mat.UnityMaterial.SetVector(args.ParamName, new UnityEngine.Vector2((float)args.Param.Vec2.X, (float)args.Param.Vec2.Y));
                break;
            case AnyVal.ValOneofCase.Vec3:
                mat.UnityMaterial.SetVector(args.ParamName, new UnityEngine.Vector3((float)args.Param.Vec3.X, (float)args.Param.Vec3.Y, (float)args.Param.Vec3.Z));
                break;
            case AnyVal.ValOneofCase.Vec4:
                mat.UnityMaterial.SetVector(args.ParamName, new UnityEngine.Vector4((float)args.Param.Vec4.W, (float)args.Param.Vec4.X, (float)args.Param.Vec4.Y, (float)args.Param.Vec4.Z));
                break;
            default:
                throw new NotImplementedException();
        }
    }
    private void MaterialSetParamC(RenderAction.Types.MaterialSetParamC args)
    {
        var mat = _components[args.MaterialId] as AbyssEngine.Component.Material;
        var comp = _components[args.ComponentId];
        switch (comp)
        {
            case AbyssEngine.Component.Image image:
                mat.SetTexture(args.ParamName, image);
                break;
            default:
                throw new NotImplementedException();
        }
    }
    private void DeleteMaterial(RenderAction.Types.DeleteMaterial args)
    {
        DeleteComponent(args.MaterialId);
    }
    private void CreateStaticMesh(RenderAction.Types.CreateStaticMesh args)
    {
        _components[args.MeshId] = new AbyssEngine.Component.StaticMesh(args.File, objHolder, "C" + args.MeshId.ToString());
    }
    private void StaticMeshSetMaterial(RenderAction.Types.StaticMeshSetMaterial args)
    {
        var mesh = _components[args.MeshId] as AbyssEngine.Component.StaticMesh;
        var mat = _components[args.MaterialId] as AbyssEngine.Component.Material;
        mesh.SetMaterial(args.MaterialSlot, mat);
    }
    private void ElemAttachStaticMesh(RenderAction.Types.ElemAttachStaticMesh args)
    {
        var parent = _game_objects[args.ElementId];
        var mesh = _components[args.MeshId] as AbyssEngine.Component.StaticMesh;
        mesh.InstantiateTracked(parent);
    }
    private void DeleteStaticMesh(RenderAction.Types.DeleteStaticMesh args)
    {
        DeleteComponent(args.MeshId);
    }
    private void CreateAnimation(RenderAction.Types.CreateAnimation args) { }
    private void DeleteAnimation(RenderAction.Types.DeleteAnimation args) { }


    //others
    private void DeleteComponent(int component_id)
    {
        _components[component_id].Dispose();
        _components.Remove(component_id);
    }

    private void LocalInfo(RenderAction.Types.LocalInfo args)
    {
        _local_aurl = args.Aurl;
        _local_hash = args.LocalHash;
        uiHandler.SetLocalAddrText(args.LocalHash);
    }
    private void InfoContentShared(RenderAction.Types.InfoContentShared args)
    {
        soms[args.ContentUuid] = args.ContentUuid + " " + args.ContentUrl + " from " + args.SharerHash + " in " + args.WorldUuid;
        SetAdditionalInfoCallback(string.Join("\n", soms.Values));
    }
    private void InfoContentDeleted(RenderAction.Types.InfoContentDeleted args)
    {
        soms.Remove(args.ContentUuid);
        SetAdditionalInfoCallback(string.Join("\n", soms.Values));
    }
    private void ConsolePrint(RenderAction.Types.ConsolePrint args)
    {
        SetAdditionalInfoCallback(args.Text);
    }
}