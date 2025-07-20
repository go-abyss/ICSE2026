using Google.Protobuf;
using static AbyssCLI.ABI.RenderAction.Types;
using System.IO;
using System;

namespace AbyssCLI.ABI
{
    public class RenderActionWriter
    {
		public RenderActionWriter(System.IO.Stream stream) {
			_out_stream = stream;
		}
		
public void CreateElement
(
    int parent_id,
    int element_id
)
=> Write(new RenderAction()
{
    CreateElement = new CreateElement
    {
        ParentId = parent_id,
        ElementId = element_id
    }
});
public void MoveElement
(
    int element_id,
    int new_parent_id
)
=> Write(new RenderAction()
{
    MoveElement = new MoveElement
    {
        ElementId = element_id,
        NewParentId = new_parent_id
    }
});
public void DeleteElement
(
    int element_id
)
=> Write(new RenderAction()
{
    DeleteElement = new DeleteElement
    {
        ElementId = element_id
    }
});
public void ElemSetPos
(
    int element_id,
    Vec3 pos,
    Vec4 rot
)
=> Write(new RenderAction()
{
    ElemSetPos = new ElemSetPos
    {
        ElementId = element_id,
        Pos = pos,
        Rot = rot
    }
});
public void CreateItem
(
    int element_id,
    string sharer_hash,
    ByteString uuid
)
=> Write(new RenderAction()
{
    CreateItem = new CreateItem
    {
        ElementId = element_id,
        SharerHash = sharer_hash,
        Uuid = uuid
    }
});
public void DeleteItem
(
    int element_id
)
=> Write(new RenderAction()
{
    DeleteItem = new DeleteItem
    {
        ElementId = element_id
    }
});
public void ItemSetIcon
(
    int element_id,
    int image_id
)
=> Write(new RenderAction()
{
    ItemSetIcon = new ItemSetIcon
    {
        ElementId = element_id,
        ImageId = image_id
    }
});
public void MemberInfo
(
    string peer_hash
)
=> Write(new RenderAction()
{
    MemberInfo = new MemberInfo
    {
        PeerHash = peer_hash
    }
});
public void MemberSetProfile
(
    int iamge_id
)
=> Write(new RenderAction()
{
    MemberSetProfile = new MemberSetProfile
    {
        IamgeId = iamge_id
    }
});
public void MemberLeave
(
    string peer_hash
)
=> Write(new RenderAction()
{
    MemberLeave = new MemberLeave
    {
        PeerHash = peer_hash
    }
});
public void CreateImage
(
    int image_id,
    File file
)
=> Write(new RenderAction()
{
    CreateImage = new CreateImage
    {
        ImageId = image_id,
        File = file
    }
});
public void DeleteImage
(
    int image_id
)
=> Write(new RenderAction()
{
    DeleteImage = new DeleteImage
    {
        ImageId = image_id
    }
});
public void CreateMaterialV
(
    int material_id,
    string shader_name
)
=> Write(new RenderAction()
{
    CreateMaterialV = new CreateMaterialV
    {
        MaterialId = material_id,
        ShaderName = shader_name
    }
});
public void CreateMaterialF
(
    int material_id,
    File file
)
=> Write(new RenderAction()
{
    CreateMaterialF = new CreateMaterialF
    {
        MaterialId = material_id,
        File = file
    }
});
public void MaterialSetParamV
(
    int material_id,
    string param_name,
    AnyVal param
)
=> Write(new RenderAction()
{
    MaterialSetParamV = new MaterialSetParamV
    {
        MaterialId = material_id,
        ParamName = param_name,
        Param = param
    }
});
public void MaterialSetParamC
(
    int material_id,
    string param_name,
    int component_id
)
=> Write(new RenderAction()
{
    MaterialSetParamC = new MaterialSetParamC
    {
        MaterialId = material_id,
        ParamName = param_name,
        ComponentId = component_id
    }
});
public void DeleteMaterial
(
    int material_id
)
=> Write(new RenderAction()
{
    DeleteMaterial = new DeleteMaterial
    {
        MaterialId = material_id
    }
});
public void CreateStaticMesh
(
    int mesh_id,
    File file
)
=> Write(new RenderAction()
{
    CreateStaticMesh = new CreateStaticMesh
    {
        MeshId = mesh_id,
        File = file
    }
});
public void StaticMeshSetMaterial
(
    int mesh_id,
    int material_slot,
    int material_id
)
=> Write(new RenderAction()
{
    StaticMeshSetMaterial = new StaticMeshSetMaterial
    {
        MeshId = mesh_id,
        MaterialSlot = material_slot,
        MaterialId = material_id
    }
});
public void ElemAttachStaticMesh
(
    int element_id,
    int mesh_id
)
=> Write(new RenderAction()
{
    ElemAttachStaticMesh = new ElemAttachStaticMesh
    {
        ElementId = element_id,
        MeshId = mesh_id
    }
});
public void DeleteStaticMesh
(
    int mesh_id
)
=> Write(new RenderAction()
{
    DeleteStaticMesh = new DeleteStaticMesh
    {
        MeshId = mesh_id
    }
});
public void CreateAnimation
(
    int animation_id,
    File file
)
=> Write(new RenderAction()
{
    CreateAnimation = new CreateAnimation
    {
        AnimationId = animation_id,
        File = file
    }
});
public void DeleteAnimation
(
    int animation_id
)
=> Write(new RenderAction()
{
    DeleteAnimation = new DeleteAnimation
    {
        AnimationId = animation_id
    }
});
public void LocalInfo
(
    string aurl,
    string local_hash
)
=> Write(new RenderAction()
{
    LocalInfo = new LocalInfo
    {
        Aurl = aurl,
        LocalHash = local_hash
    }
});
public void InfoContentShared
(
    string content_uuid,
    string content_url,
    string sharer_hash,
    string world_uuid
)
=> Write(new RenderAction()
{
    InfoContentShared = new InfoContentShared
    {
        ContentUuid = content_uuid,
        ContentUrl = content_url,
        SharerHash = sharer_hash,
        WorldUuid = world_uuid
    }
});
public void InfoContentDeleted
(
    string content_uuid,
    string sharer_hash,
    string world_uuid
)
=> Write(new RenderAction()
{
    InfoContentDeleted = new InfoContentDeleted
    {
        ContentUuid = content_uuid,
        SharerHash = sharer_hash,
        WorldUuid = world_uuid
    }
});
public void ConsolePrint
(
    string text
)
=> Write(new RenderAction()
{
    ConsolePrint = new ConsolePrint
    {
        Text = text
    }
});

		public void Flush()
		{
			_out_stream.Flush();
		}

		private void Write(RenderAction msg)
		{
			var msg_len = msg.CalculateSize();
			_out_sema.WaitOne();
			_out_stream.Write(BitConverter.GetBytes(msg_len));
			msg.WriteTo(_out_stream);
			_out_sema.Release();
            if(AutoFlush)
            {
                _out_stream.Flush();
            }
		}
		public bool AutoFlush = false;
		private readonly System.IO.Stream _out_stream;
		private readonly System.Threading.Semaphore _out_sema = new(1, 1);
	}
}