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
            return GetOrCreateLayer(database, transaction, layerName, color, LineWeight.ByLayer);
        }

        public static ObjectId GetOrCreateLayer(
            Database database,
            Transaction transaction,
            string layerName,
            Color color,
            LineWeight lineWeight)
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
                LineWeight = lineWeight,
            };

            layerTable.UpgradeOpen();
            ObjectId layerId = layerTable.Add(layer);
            transaction.AddNewlyCreatedDBObject(layer, true);
            return layerId;
        }
    }
}