using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using TestDrawing.Drawing;

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
                if (arrowBlockId.IsValid)
                {
                    existing.Dimtsz = 0;
                    existing.Dimblk1 = arrowBlockId;
                    existing.Dimblk2 = arrowBlockId;
                }
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
                existing.Dimdec = 0;
                existing.Dimtix = false;
                existing.Dimtoh = false;
                existing.Dimtih = false;
                existing.Dimlwd = LineWeight.ByLayer;
                existing.Dimclre = lineColor;

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
                Dimgap = 20,
                Dimdec = 0,
                Dimtix = false,
                Dimtoh = false,
                Dimtih = false,
                Dimclre = lineColor
            };
            if (arrowBlockId.IsValid)
            {
                style.Dimtsz = 0;
                style.Dimblk1 = arrowBlockId;
                style.Dimblk2 = arrowBlockId;
            }

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