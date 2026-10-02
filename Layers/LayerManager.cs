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
            return GetOrCreateLayer(database, transaction, layerName, color, LineWeight.ByLayer, null);
        }

        public static ObjectId GetOrCreateLayer(
            Database database,
            Transaction transaction,
            string layerName,
            Color color,
            LineWeight lineWeight)
        {
            return GetOrCreateLayer(database, transaction, layerName, color, lineWeight, null);
        }

        public static ObjectId GetOrCreateLayer(
            Database database,
            Transaction transaction,
            string layerName,
            Color color,
            LineWeight lineWeight,
            Transparency? transparency)
        {
            LayerTable layerTable = (LayerTable)transaction.GetObject(database.LayerTableId, OpenMode.ForRead);

            if (layerTable.Has(layerName))
            {
                LayerTableRecord existingLayer =
                    (LayerTableRecord)transaction.GetObject(
                        layerTable[layerName],
                        transparency.HasValue ? OpenMode.ForWrite : OpenMode.ForRead);

                if (transparency.HasValue)
                    existingLayer.Transparency = transparency.Value;

                return existingLayer.ObjectId;
            }

            LayerTableRecord layer = new LayerTableRecord
            {
                Name = layerName,
                Color = color,
                LineWeight = lineWeight
            };

            layerTable.UpgradeOpen();
            ObjectId layerId = layerTable.Add(layer);
            transaction.AddNewlyCreatedDBObject(layer, true);

            if (transparency.HasValue)
                layer.Transparency = transparency.Value;

            return layerId;
        }
    }
}