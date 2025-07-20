using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocalItemSection
{
    public readonly VisualElement visualElement;
    private readonly Dictionary<int, ItemIcon> items;
    private readonly Texture2D defaultIcon;
    public LocalItemSection(VisualElement visualElement, Texture2D defaultIcon)
    {
        this.visualElement = visualElement;
        this.items = new();
        this.defaultIcon = defaultIcon;
    }
    public void CreateItem(Executor executor, int element_id, Guid uuid)
    {
        var item = new ItemIcon(uuid, defaultIcon);
        items[element_id] = item;
        item.RegisterCloseCallback(() =>
        {
            executor.UnshareContent(uuid);
        });
        Show();
    }
    public void UpdateIcon(int element_id, Texture2D icon)
    {
        var existing_item = items[element_id];
        existing_item.style.backgroundImage = icon;
        Show();
    }
    public void RemoveItem(int element_id)
    {
        items.Remove(element_id);
        Show();
    }
    public void Show()
    {
        visualElement.Clear();
        foreach (var item in items.Values)
        {
            visualElement.Add(item);
        }
    }
}