using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private Executor executor;
    [SerializeField] public Texture2D defaultItemIcon;
    [SerializeField] public Texture2D defaultMemberProfile;

    public Func<UnityEngine.Transform> GetContentSpawnPos;

    private VisualElement root;
    private TextField addressBar;
    private TextField sub_addressBar;
    private Label localAddrLabel;
    private Label extraLabel; //TODO

    public LocalItemSection LocalItemSection;
    public MemberItemSection MemberItemSection;
    public MemberProfileSection MemberProfileSection;

    void Awake()
    {
        root = uiDocument.rootVisualElement;

        addressBar = UQueryExtensions.Q<TextField>(root, "address-bar");
        addressBar.RegisterCallback<KeyDownEvent>((x) =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                AddressBarSubmit(addressBar.value);
            }
        });

        sub_addressBar = UQueryExtensions.Q<TextField>(root, "sub-address-bar");
        sub_addressBar.RegisterCallback<KeyDownEvent>((x) =>
        {
            if (x.keyCode == KeyCode.Return)
            {
                SubAddressBarSubmit(sub_addressBar.value);
            }
        });

        localAddrLabel = UQueryExtensions.Q<Label>(root, "info");

        extraLabel = UQueryExtensions.Q<Label>(root, "info-more");
        executor.SetAdditionalInfoCallback = (info) => { extraLabel.text = info; };

        LocalItemSection = new(UQueryExtensions.Q(root, "itembar"), defaultItemIcon);

        MemberItemSection = new(UQueryExtensions.Q(root, "memberitemsection"), defaultItemIcon);

        MemberProfileSection = new(UQueryExtensions.Q(root, "memberprofilesection"), defaultMemberProfile);
        MemberProfileSection.RegisterClickCallback((string peer_hash) =>
        {
            MemberItemSection.Show(peer_hash);
        });

        if (localAddrLabel == null || extraLabel == null)
        {
            Debug.LogError("UI components not found!");
        }

        Deactivate();
    }
    public void Activate()
    {
        root.visible = true;
        addressBar.focusable = true;
    }
    public void Deactivate()
    {
        MemberItemSection.Hide();
        root.visible = false;
        addressBar.focusable = false;
    }
    void AddressBarSubmit(string address)
    {
        executor.MoveWorld(address);
    }
    void SubAddressBarSubmit(string address)
    {
        if (address.StartsWith("connect "))
        {
            var conn_addr = address["connect ".Length..];
            executor.ConnectPeer(conn_addr);
            return;
        }
        var transform = GetContentSpawnPos();
        var uuid = Guid.NewGuid();
        executor.ShareContent(uuid, address, transform.position, transform.rotation);
    }
    public void SetLocalAddrText(string text)
    {
        localAddrLabel.text = text;
    }
}
