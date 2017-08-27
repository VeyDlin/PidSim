using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PidSim.Model {
    class PidRegulator {

        private PidParameters pidParameters;
        private PidData pidData;

        public struct PidParameters {
            public float Kr;   // reference set-point weighting
            public float Kp;   // proportional loop gain
            public float Ki;   // integral gain
            public float Kd;   // derivative gain
            public float Km;   // derivative weighting
            public float Umax; // upper saturation limit
            public float Umin; // lower saturation limit
        };

        private struct PidData {
            public float up; // proportional term
            public float ui; // integral term
            public float ud; // derivative term
            public float v1; // pre-saturated controller output
            public float i1; // integrator storage: ui(k-1)
            public float d1; // differentiator storage: ud(k-1)
            public float d2; // differentiator storage: d2(k-1)
            public float w1; // saturation record: [u(k-1) - v(k-1)]
            public float c1; // derivative filter coefficient 1
            public float c2; // derivative filter coefficient 2
        }




        // Инициализация
        public PidRegulator(PidParameters _parameters) {
            pidParameters = _parameters;
            Reset();
        }




        // Тик ПИДа
        // feedbackVal - текущее значение системы
        // referenceVal - значение системы, к которому мы стремимся
        public float Tick(float feedbackVal, float referenceVal) {
            float outputVal; // Выход ПИД

            // proportional term
            pidData.up = (pidParameters.Kr * referenceVal) - feedbackVal;

            // integral term
            pidData.ui = (pidParameters.Ki * (pidData.w1 * (referenceVal - feedbackVal))) + pidData.i1;
            pidData.i1 = pidData.ui;

            // derivative term
            pidData.d2 = (pidParameters.Kd * (pidData.c1 * ((referenceVal * pidParameters.Km) - feedbackVal))) - pidData.d2;
            pidData.ud = pidData.d2 + pidData.d1;
            pidData.d1 = (pidData.ud * pidData.c2);

            // control output
            pidData.v1 = (pidParameters.Kp * (pidData.up + pidData.ui + pidData.ud));
            outputVal = Math.Max((Math.Min(pidParameters.Umax, pidData.v1)), pidParameters.Umin);
            pidData.w1 = (outputVal == pidData.v1) ? 1.0f : 0.0f;

            return outputVal;
        }





        // Сброс параметров которым успел обучится ПИД
        public void Reset() {
            pidData.up = 0.0f;
            pidData.ui = 0.0f;
            pidData.ud = 0.0f;
            pidData.v1 = 0.0f;
            pidData.i1 = 0.0f;
            pidData.d1 = 0.0f;
            pidData.d2 = 0.0f;
            pidData.w1 = 1.0f;
            pidData.c1 = 0;
            pidData.c2 = 0;
        }
    }
}
