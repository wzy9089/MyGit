using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koga.Paint
{
    public class PaintToolManager
    {
        private IPaintControl owner;
        private Dictionary<string,PaintTool> paintToolDict = new Dictionary<string,PaintTool>();

        public PaintTool CurrentTool { get; private set; }

        public event EventHandler? CurrentToolChanged;

        internal PaintToolManager(IPaintControl ownerCtl)
        {
            Debug.Assert(ownerCtl != null);

            owner = ownerCtl;

            LoadPaintTools();

            SetCurrentTool(paintToolDict.Keys.First());
        }

        public void SetCurrentTool(string toolID)
        {
            if (paintToolDict.ContainsKey(toolID))
            {
                if (CurrentTool != null)
                {
                    if (CurrentTool.ToolID == toolID)
                    {
                        return;
                    }
                    else
                    {
                        CurrentTool.PaintStarted -= owner.OnPaintStarted;
                        CurrentTool.PaintFinished -= owner.OnPaintFinished;
                    }
                }

                CurrentTool = paintToolDict[toolID];
                CurrentTool.PaintStarted += owner.OnPaintStarted;
                CurrentTool.PaintFinished += owner.OnPaintFinished;

                OnCurrentToolChanged();
            }
        }

        public PaintTool? GetPaintTool(string toolID)
        {
            if (paintToolDict.ContainsKey(toolID))
            {
                return paintToolDict[toolID];
            }
         
            return null;
        }

        public bool ContainsPaintTool(string toolID)
        {
            return paintToolDict.ContainsKey(toolID);
        }

        private void OnCurrentToolChanged()
        {
            CurrentToolChanged?.Invoke(this, EventArgs.Empty);
        }

        private void LoadPaintTools()
        {
            AddPaintTool(new StrokePaintTool(owner));
        }

        private void AddPaintTool(PaintTool paintTool)
        {
            if(paintToolDict.ContainsKey(paintTool.ToolID))
            {
                throw new ArgumentException("PaintTool with the same ID already exists");
            }

            paintToolDict.Add(paintTool.ToolID, paintTool);
        }
    }
}
