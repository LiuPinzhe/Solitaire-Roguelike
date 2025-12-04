using UnityEngine;
using UnityEngine.UI;

public class AutoSetupDropZones : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private Transform[] tableauColumns = new Transform[7];
    [SerializeField] private Transform[] foundationPiles = new Transform[4];
    
    [ContextMenu("Setup Drop Zones")]
    public void SetupDropZones()
    {
        // 设置Tableau列
        for (int i = 0; i < tableauColumns.Length; i++)
        {
            if (tableauColumns[i] != null)
            {
                SetupTableauColumn(tableauColumns[i], i);
            }
        }
        
        // 设置Foundation堆
        for (int i = 0; i < foundationPiles.Length; i++)
        {
            if (foundationPiles[i] != null)
            {
                SetupFoundationPile(foundationPiles[i], i);
            }
        }
        
        Debug.Log("Drop zones setup completed!");
    }
    
    void SetupTableauColumn(Transform column, int index)
    {
        // 添加DropZone组件
        DropZone dropZone = column.GetComponent<DropZone>();
        if (dropZone == null)
        {
            dropZone = column.gameObject.AddComponent<DropZone>();
        }
        
        // 设置DropZone参数
        var zoneTypeField = typeof(DropZone).GetField("zoneType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var zoneIndexField = typeof(DropZone).GetField("zoneIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        zoneTypeField.SetValue(dropZone, DropZone.ZoneType.Tableau);
        zoneIndexField.SetValue(dropZone, index);
        
        // 添加Image组件用于接收射线检测
        Image image = column.GetComponent<Image>();
        if (image == null)
        {
            image = column.gameObject.AddComponent<Image>();
        }
        
        // 设置为透明但可接收射线
        Color color = image.color;
        color.a = 0f;
        image.color = color;
        image.raycastTarget = true;
        
        Debug.Log($"Setup Tableau Column {index}: {column.name}");
    }
    
    void SetupFoundationPile(Transform foundation, int index)
    {
        // 添加DropZone组件
        DropZone dropZone = foundation.GetComponent<DropZone>();
        if (dropZone == null)
        {
            dropZone = foundation.gameObject.AddComponent<DropZone>();
        }
        
        // 设置DropZone参数
        var zoneTypeField = typeof(DropZone).GetField("zoneType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var zoneIndexField = typeof(DropZone).GetField("zoneIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        zoneTypeField.SetValue(dropZone, DropZone.ZoneType.Foundation);
        zoneIndexField.SetValue(dropZone, index);
        
        // 添加Image组件
        Image image = foundation.GetComponent<Image>();
        if (image == null)
        {
            image = foundation.gameObject.AddComponent<Image>();
        }
        
        // 设置为透明但可接收射线
        Color color = image.color;
        color.a = 0f;
        image.color = color;
        image.raycastTarget = true;
        
        Debug.Log($"Setup Foundation Pile {index}: {foundation.name}");
    }
}