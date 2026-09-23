using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace TestDrawing.Layers
{
    public static class LayerManager
    {
        public static ObjectId GetOrCreateLayer(
            Database database,
            Transaction transaction,
            string layerName,
            Color color)
        {
            LayerTable layerTable = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

            if (layerTable.Has(layerName))
            {
                return layerTable[layerName];
            }

            LayerTableRecord layer = new LayerTableRecord
            {
                Name = layerName,
                Color = color,
                LineWeight = LineWeight.LineWeight050,
            };

            layerTable.UpgradeOpen();
            ObjectId layerId = layerTable.Add(layer);
            transaction.AddNewlyCreatedDBObject(layer, true);
            return layerId;
        }
    }
}