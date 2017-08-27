using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using PidSim.ViewModel.Properties;
using PidSim.Model;
using System.Windows.Threading;





namespace PidSim.ViewModel {
    [ImplementPropertyChanged]
    class MainWindowViewModel {


        // Свойства
        public SystemGraph systemGraph { get; set; }
        public HeatGraph heatGraph { get; set; }


        // Приватные глобальные свойства VM 
        private ThermalSystem thermalSystem;
        private PidRegulator pidRegulator;
        private DispatcherTimer thermalSystemTimer;




        public MainWindowViewModel() {

            // Отображение графика
            systemGraph = new SystemGraph();
            heatGraph = new HeatGraph();


            // ПИД регулятор
            PidRegulator.PidParameters pidSettings;
            pidSettings.Kr = 1.0f;
            pidSettings.Kp = 0.005f;
            pidSettings.Ki = 1.5f;
            pidSettings.Kd = 0.0f; // Эти две...
            pidSettings.Km = 1.0f; // ...лучше не трогать
            pidSettings.Umax = 500; // Максимальный выход ПИД
            pidSettings.Umin = 1;   // Минимальный выход ПИД
            pidRegulator = new PidRegulator(pidSettings);



            // Тепловая система
            thermalSystem = new ThermalSystem() {
                TargetTemperature = 200, // Температура к которой мы стремимся
            };
            thermalSystem.control += new ThermalSystem.ThermalSystemControlHandler(ThermalSystemСontrol);

            // Тик тепловой системы
            thermalSystemTimer = new DispatcherTimer();
            thermalSystemTimer.Interval = TimeSpan.FromMilliseconds(20);
            thermalSystemTimer.Tick += delegate (object sender, EventArgs e){
                thermalSystem.Integrate();
            };
            thermalSystemTimer.Start();
        }







        // Запрос мощности (функция должна возвращать новую мощность для печи)
        double ThermalSystemСontrol(double currentTemperature, double targetTemperature, double timeDelta) {
            // Вход на систему (сколько мощности мы даем системе)
            float systemInputPower = pidRegulator.Tick((float)currentTemperature, (float)targetTemperature);

            // Обновляем графики
            systemGraph.AddSystemTick(currentTemperature, targetTemperature, systemInputPower);
            heatGraph.SetHeatMapData(thermalSystem.temp);


            return systemInputPower;
        }


    }

}
