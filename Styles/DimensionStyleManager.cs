using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace TestDrawing.Styles
{
    public static class DimensionStyleManager
    {
        public static ObjectId GetOrCreate(
            Database database,
            Transaction transaction,
            string name,
            ObjectId textStyleId,
            double textHeight,
            double arrowSize,
            string arrowBlockName,
            Color textColor,
            Color lineColor)
        {
            DimStyleTable dimStyleTable = (DimStyleTable)transaction.GetObject(database.DimStyleTableId, OpenMode.ForRead);

            ObjectId arrowBlockId = GetArrowBlockId(database, transaction, arrowBlockName);

            if (dimStyleTable.Has(name))
            {
                ObjectId existingId = dimStyleTable[name];
                DimStyleTableRecord existing = (DimStyleTableRecord)transaction.GetObject(existingId, OpenMode.ForWrite);

                existing.Dimtxsty = textStyleId;
                existing.Dimtxt = textHeight;
                existing.Dimasz = arrowSize;
                existing.Dimtsz = arrowBlockId.IsValid ? 0 : arrowSize;
                if (arrowBlockId.IsValid)
                    existing.Dimblk = arrowBlockId;
                existing.Dimclrt = textColor;
                existing.Dimclrd = lineColor;
                existing.Dimse1 = false;
                existing.Dimse2 = false;
                existing.DimfxlenOn = true;
                existing.Dimfxlen = 100;
                existing.Dimdle = 100;
                existing.Dimexe = 100;
                existing.Dimgap = 20;
                existing.Dimtad = 1;
                existing.Dimlwd = LineWeight.ByLayer;

                return existingId;
            }

            DimStyleTableRecord style = new DimStyleTableRecord()
            {
                Name = name,
                Dimtxsty = textStyleId,
                Dimtxt = textHeight,
                Dimasz = arrowSize,
                Dimclrt = textColor,
                Dimlwd = LineWeight.ByLayer,
                DimfxlenOn = true,
                Dimclrd = lineColor,
                Dimse1 = true,
                Dimse2 = true,
                Dimtad = 1,
                Dimgap = 20
            };
            if (arrowBlockId.IsValid)
                style.Dimblk = arrowBlockId;
            else
                style.Dimtsz = arrowSize;

            dimStyleTable.UpgradeOpen();

            ObjectId styleId = dimStyleTable.Add(style);
            transaction.AddNewlyCreatedDBObject(style, true);

            return styleId;
        }

        private static ObjectId GetArrowBlockId(Database database, Transaction transaction, string arrowBlockName)
        {
            if (string.IsNullOrEmpty(arrowBlockName))
                return ObjectId.Null;

            string[] candidateNames = { arrowBlockName, arrowBlockName.TrimStart('_'), "ArchTick", "_ARCHTICK" };

            BlockTable blockTable = (BlockTable)transaction.GetObject(database.BlockTableId, OpenMode.ForRead);

            foreach (string candidate in candidateNames)
            {
                if (blockTable.Has(candidate))
                    return blockTable[candidate];
            }

            return ObjectId.Null;
        }
    }
}