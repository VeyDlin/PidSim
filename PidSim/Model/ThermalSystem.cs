using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PidSim.Model {
    class ThermalSystem {
        /// <summary>
        /// Control event delegate
        /// </summary>
        /// <param name="Temperature">Current measured temperature</param>
        /// <param name="TargetTemperature">Target temperature (user control)</param>
        /// <param name="timeDelta">time delta, since prev call</param>
        /// <returns>Heater power, percent</returns>
        public delegate double ThermalSystemControlHandler(double Temperature, double TargetTemperature, double timeDelta);


        /// <summary>
        /// Control event
        /// </summary>
        public event ThermalSystemControlHandler control;

        /// <summary>
        /// Мощность нагревателя в процентах
        /// </summary>
        public double Heater {
            set {
                _heater = Math.Min(100, Math.Max(0, value));
            }
        }
        private double _heater = 0;

        /// <summary>
        /// Температура датчика
        /// </summary>
        public double Sensor {
            get { return temp[sensor_y, sensor_x]; }
        }


        /// <summary>
        /// Размер сетки модели
        /// </summary>
        public const int size = 5;

        /// <summary>
        /// Положение сенсора
        /// </summary>
        public int sensor_x = size / 2, sensor_y = 0;

        /// <summary>
        /// Потери тепла через стенки печи
        /// </summary>
        public const double Loss = 10.0f;

        /// <summary>
        /// Теплопроводность среды
        /// </summary>
        public const double Conduct = 0.8f * size;


        /// <summary>
        /// Мощность нагревателя в попугаях
        /// </summary>
        public const double HeaterPower = 10;


        /// <summary>
        /// Сетка значений температуры
        /// </summary>
        public double[,] temp = new double[size, size];

        /// <summary>
        /// Сетка значений температуры (временная копия)
        /// </summary>
        public double[,] temp1 = new double[size, size];


        public double RoomTemperature = 0;

        public double TargetTemperature = 0;

        public double timeDelta = 1.0f / 30.0f;


        public bool check(double integrateTime, ref double stabTime, ref double overReg, out double cost, double mu1 = 1, double mu2 = 1.0f) {
            cost = 0;
            reset();

            while(time < integrateTime) {
                cost += mu1 * time * time * (double)Math.Pow((TargetTemperature - Sensor) / TargetTemperature, 2);
                Integrate();
            }
            stabTime = time - timeStab;
            overReg = overControl;
            return (timeStab > 3.0);
        }


        double overControl = 0;
        double time = 0;
        double timeStab = 0;

        public void Integrate() {
            if(control != null)
                Heater = control(Sensor, TargetTemperature, timeDelta);

            int NSteps = 10;
            for(int i = 0; i < NSteps; ++i)
                IntegrateSingleStep(timeDelta / NSteps);

            time += timeDelta;
            timeStab += timeDelta;
            if(Sensor > TargetTemperature)
                overControl = (Sensor - TargetTemperature) / TargetTemperature;
            if(Math.Abs(Sensor - TargetTemperature) > 0.05 * TargetTemperature)
                timeStab = 0;
        }


        /// <summary>
        /// Шаг интегрирования модели
        /// </summary>
        private void IntegrateSingleStep(double timeDelta) {
            for(int i = 0; i < temp.GetLength(0); ++i) {
                temp[i, 0] = temp[i, 1]
                    - timeDelta * Loss * (temp[i, 0] - RoomTemperature);
                temp[i, temp.GetLength(0) - 1] = temp[i, temp.GetLength(0) - 2]
                    - timeDelta * Loss * (temp[i, temp.GetLength(0) - 1] - RoomTemperature);
            }
            for(int i = 0; i < temp.GetLength(1); ++i) {
                temp[0, i] = temp[1, i]
                    - timeDelta * Loss * (temp[1, 0] - RoomTemperature);
                if(i >= size / 5 && i < size - size / 5)
                    temp[temp.GetLength(0) - 1, i] = _heater * HeaterPower + RoomTemperature;
                else
                    temp[temp.GetLength(0) - 1, i] = temp[temp.GetLength(0) - 2, i]
                        - timeDelta * Loss * (temp[temp.GetLength(0) - 2, i] - RoomTemperature);
            }

            for(int i = 1; i < temp.GetLength(0) - 1; ++i) {
                for(int j = 1; j < temp.GetLength(1) - 1; ++j) {
                    double laplace = (temp[i + 1, j] - temp[i, j]) - (temp[i, j] - temp[i - 1, j])
                                   + (temp[i, j + 1] - temp[i, j]) - (temp[i, j] - temp[i, j - 1]);
                    temp1[i, j] = temp[i, j] + timeDelta * Conduct * laplace;
                }
            }
            var t = temp1;
            temp1 = temp;
            temp = t;
        }

        /// <summary>
        /// Сброс модели в начальное состояние
        /// </summary>
        public void reset() {
            overControl = 0;
            time = 0;
            timeStab = 0;

            for(int i = 0; i < temp.GetLength(0); ++i)
                for(int j = 0; j < temp.GetLength(1); ++j) {
                    temp[i, j] = RoomTemperature;
                    temp1[i, j] = RoomTemperature;
                }
        }

    };

}

