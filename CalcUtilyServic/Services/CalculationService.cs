using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcUtilyServic.Services
{
    public class CalculationService
    {
        // Тарифы
        private const decimal HwsRate = 35.78m;      
        private const decimal GwsRate = 35.78m;      
        private const decimal HeatNorm = 0.05349m;   // Норма тепла на 1 м³ горячей воды
        private const decimal HeatRate = 998.69m;    
        private const decimal ElecDayRate = 4.9m;   
        private const decimal ElecNightRate = 2.31m; 

        // Нормы потребления воды на человека в месяц
        private const decimal HwsNorm = 4.85m; 
        private const decimal GwsNorm = 4.01m; 

        public decimal CalculateTotal(
            bool hasHwsMeter,      // Есть ли счётчик горячей воды
            decimal hwsPrev,       // Предыдущее показание счётчика ГВС
            decimal hwsCurr,       // Текущее показание счётчика ГВС
            bool hasGwsMeter,      // Есть ли счётчик холодной воды
            decimal gwsPrev,       // Предыдущее показание счётчика ХВС
            decimal gwsCurr,       // Текущее показание счётчика ХВС
            bool hasElecMeter,     // Есть ли счётчик электроэнергии
            decimal edPrev,        // Предыдущее показание дневного тарифа
            decimal edCurr,        // Текущее показание дневного тарифа
            decimal enPrev,        // Предыдущее показание ночного тарифа
            decimal enCurr,        // Текущее показание ночного тарифа
            int residentsFirstHalf,   // Кол-во жильцов в первой половине месяца
            int residentsSecondHalf) // Кол-во жильцов во второй половине месяца
        {
            // Расход горячей воды (по счётчику или по норме)
            decimal hwsVolume = hasHwsMeter
                ? (hwsCurr - hwsPrev)
                : CalculateNormVolume(residentsFirstHalf, residentsSecondHalf, HwsNorm);

            // Расход холодной воды (по счётчику или по норме)
            decimal gwsVolume = hasGwsMeter
                ? (gwsCurr - gwsPrev)
                : CalculateNormVolume(residentsFirstHalf, residentsSecondHalf, GwsNorm);

            // Расход электроэнергии по дневному тарифу
            decimal elecDayVolume = hasElecMeter
                ? (edCurr - edPrev)
                : 0;

            // Расход электроэнергии по ночному тарифу
            decimal elecNightVolume = hasElecMeter
                ? (enCurr - enPrev)
                : 0;

            // Сумма за горячую воду
            decimal hwsAmount = hwsVolume * HwsRate;

            // Сумма за холодную воду
            decimal gwsWaterAmount = gwsVolume * GwsRate;

            // Сумма за подогрев воды (тепло на 1 м³ * тариф тепла)
            decimal gwsHeatAmount = gwsVolume * HeatNorm * HeatRate;

            // Сумма за дневное электричество
            decimal elecDayAmount = elecDayVolume * ElecDayRate;

            // Сумма за ночное электричество
            decimal elecNightAmount = elecNightVolume * ElecNightRate;

            // Общая сумма
            return hwsAmount + gwsWaterAmount + gwsHeatAmount + elecDayAmount + elecNightAmount;
        }

        // Расчёт объема воды по нормам, если нет счётчиков
        private decimal CalculateNormVolume(int firstHalfResidents, int secondHalfResidents, decimal normPerPerson)
        {
            // Расход за 15 дней (первая половина месяца)
            decimal firstHalfVolume = firstHalfResidents * normPerPerson * 15m / 30m;

            // Расход за 16 дней (вторая половина месяца)
            decimal secondHalfVolume = secondHalfResidents * normPerPerson * 16m / 30m;

            return firstHalfVolume + secondHalfVolume;
        }
    }

}
