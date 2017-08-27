using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using PropertyChanged;

namespace PidSim.ViewModel.Properties {
    [ImplementPropertyChanged]
    class HeatGraph {

        public PlotModel plotModel { get; set; }

        private LinearColorAxis linearColorAxis;
        private LinearAxis linearAxisX, linearAxisY;
        private HeatMapSeries heatMapSeries;





        // Инициализация
        public HeatGraph() {
            plotModel = new PlotModel { };

            // Линия температурной карты
            plotModel.Axes.Add(linearColorAxis = new LinearColorAxis() {
                Position = AxisPosition.Top,
                MaximumRange = 1000,
                MinimumRange = 0,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1000,
                IsZoomEnabled = false
            });

            // Ось X
            plotModel.Axes.Add(linearAxisX = new LinearAxis() {
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false
            });

            // Ось Y
            plotModel.Axes.Add(linearAxisY = new LinearAxis() {
                Position = AxisPosition.Left,
                IsZoomEnabled = false
            });

            // Температурная карта
            plotModel.Series.Add(heatMapSeries = new HeatMapSeries() {
                X0 = 0,
                X1 = 1000,
                Y0 = 0,
                Y1 = 1000,
            });

            SetHeatMapData(new double[2, 2] { { 0, 0 }, { 0, 0 } });
        }





        public void SetHeatMapData(double[,] data) {
            heatMapSeries.ClearSelection();
            heatMapSeries.Data = data;

            plotModel.InvalidatePlot(true);
        }


    }
}
