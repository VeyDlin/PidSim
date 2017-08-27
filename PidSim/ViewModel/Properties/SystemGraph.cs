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
    class SystemGraph {


        public PlotModel plotModel { get; set; }
        private LinearAxis linearAxisX, linearAxisY;
        private LineSeries lineCurTemp, lineSystemPower;
        private StairStepSeries lineTargetTemp;


        // Точки построения линий на графике
        private List<DataPoint> pointsCurTemp = new List<DataPoint>(); // Текущая температура 
        private List<DataPoint> pointsTargetTemp = new List<DataPoint>(); // Температура, к которой мы стремимся
        private List<DataPoint> pointsSystemPower = new List<DataPoint>(); // Мощность, которую вкачали в систему

        // Шаг по оси X
        private int stepAxesX;




        // Инициализация
        public SystemGraph() {
            stepAxesX = 0;

            plotModel = new PlotModel { };

            // Ось X
            plotModel.Axes.Add(linearAxisX = new LinearAxis() {
                MajorGridlineStyle = LineStyle.Solid,
                MaximumPadding = 0,
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                MinimumRange = 1000,
                MaximumRange = 1000,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom
            });

            // Ось Y
            plotModel.Axes.Add(linearAxisY = new LinearAxis() {
                MajorGridlineStyle = LineStyle.Solid,
                MaximumPadding = 0,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 1000,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Left
            });



            // Линия текущей температуры
            plotModel.Series.Add(lineCurTemp = new LineSeries() {
                Title = "Текущая температура",
            });

            // Линия температуры, к которой мы стремимся
            plotModel.Series.Add(lineTargetTemp = new StairStepSeries() {
                Title = "Нужная температура",
                VerticalStrokeThickness = 0.4
            });

            // Линия мощности которую вкачали в систему
            plotModel.Series.Add(lineSystemPower = new LineSeries() {
                Title = "Мощность"
            });
        }





        // Добавить состояние системы в момент времени
        public void AddSystemTick(double curTemp, double targetTemp, double systemPower) {
            pointsCurTemp.Add(new DataPoint(stepAxesX, curTemp));
            pointsTargetTemp.Add(new DataPoint(stepAxesX, targetTemp));
            pointsSystemPower.Add(new DataPoint(stepAxesX, systemPower));

            stepAxesX++;

            Update();
        }





        // Обновить все нахер
        private void Update() {
            lineCurTemp.Points.Clear();
            lineTargetTemp.Points.Clear();
            lineSystemPower.Points.Clear();

            lineCurTemp.Points.AddRange(pointsCurTemp);
            lineTargetTemp.Points.AddRange(pointsTargetTemp);
            lineSystemPower.Points.AddRange(pointsSystemPower);

            plotModel.InvalidatePlot(true);
        }





        // Сбросить все нахер
        public void Reset() {
            stepAxesX = 0;

            pointsCurTemp.Clear();
            pointsTargetTemp.Clear();
            pointsSystemPower.Clear();

            Update();
        }
    }
}
