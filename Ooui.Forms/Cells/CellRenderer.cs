﻿using Ooui.Forms.Extensions;
using System;
using Xamarin.Forms;

namespace Ooui.Forms.Cells
{
    public class CellRenderer : IRegisterable
    {
        private EventHandler _onForceUpdateSizeRequested;

        static readonly BindableProperty RealCellProperty =
            BindableProperty.CreateAttached("RealCell", typeof(Div),
                typeof(Cell), null);

        public virtual CellView GetCell(Cell item, CellView reusableView, List listView)
        {
            var nativeCell = reusableView as CellView ?? GetCellInstance(item);

            nativeCell.Cell = item;

            WireUpForceUpdateSizeRequested(item, nativeCell);
            UpdateBackground(nativeCell, item);

            return nativeCell;
        }

        internal static CellView GetRealCell(BindableObject cell)
        {
            return (CellView)cell.GetValue(RealCellProperty);
        }

        internal static void SetRealCell(BindableObject cell, CellView renderer)
        {
            cell.SetValue(RealCellProperty, renderer);
        }

        protected virtual CellView GetCellInstance(Cell item)
        {
            return new CellView();
        }

        protected virtual void OnForceUpdateSizeRequest(Cell cell, CellView nativeCell)
        {
            nativeCell.Style.Height = (int)cell.RenderHeight;
        }

        protected void UpdateBackground(CellView tableViewCell, Cell cell)
        {
            var defaultColor = Xamarin.Forms.Color.White.ToOouiColor();
            Color backgroundColor = defaultColor;

            if (cell.RealParent is VisualElement element)
                backgroundColor = element.BackgroundColor ==
                    Xamarin.Forms.Color.Default ? backgroundColor : element.BackgroundColor.ToOouiColor();
            
            tableViewCell.Style.BackgroundColor = backgroundColor;
        }

        protected void WireUpForceUpdateSizeRequested(Cell cell, CellView nativeCell)
        {
            cell.ForceUpdateSizeRequested -= _onForceUpdateSizeRequested;

            _onForceUpdateSizeRequested = (sender, e) =>
            {
                OnForceUpdateSizeRequest(cell, nativeCell);
            };

            cell.ForceUpdateSizeRequested += _onForceUpdateSizeRequested;
        }
    }
}
