using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcUtilyServic.Services
{
    public class CalculationService
    {
        private const decimal HwsRate = 35.78m;
        private const decimal GwsRate = 35.78m;
        private const decimal HeatNorm = 0.05349m; 
        private const decimal HeatRate = 998.69m; 
        private const decimal ElecDayRate = 4.9m;
        private const decimal ElecNightRate = 2.31m;

        private const decimal HwsNorm = 4.85m; 
        private const decimal GwsNorm = 4.01m;

        public decimal CalculateTotal(
            bool hasHwsMeter, decimal hwsPrev, decimal hwsCurr,
            bool hasGwsMeter, decimal gwsPrev, decimal gwsCurr,
            bool hasElecMeter,
            decimal edPrev, decimal edCurr,
            decimal enPrev, decimal enCurr,
            int residentsFirstHalf, int residentsSecondHalf)
        {
            decimal hwsVolume = hasHwsMeter ? (hwsCurr - hwsPrev) : CalculateNormVolume(residentsFirstHalf, residentsSecondHalf, HwsNorm);
            decimal gwsVolume = hasGwsMeter ? (gwsCurr - gwsPrev) : CalculateNormVolume(residentsFirstHalf, residentsSecondHalf, GwsNorm);

            decimal elecDayVolume = hasElecMeter ? (edCurr - edPrev) : 0;  
            decimal elecNightVolume = hasElecMeter ? (enCurr - enPrev) : 0;

            decimal hwsAmount = hwsVolume * HwsRate;

            decimal gwsWaterAmount = gwsVolume * GwsRate;
            decimal gwsHeatAmount = gwsVolume * HeatNorm * HeatRate;

            decimal elecDayAmount = elecDayVolume * ElecDayRate;
            decimal elecNightAmount = elecNightVolume * ElecNightRate;

            return hwsAmount + gwsWaterAmount + gwsHeatAmount + elecDayAmount + elecNightAmount;
        }

        private decimal CalculateNormVolume(int firstHalfResidents, int secondHalfResidents, decimal normPerPerson)
        {
            decimal firstHalfVolume = firstHalfResidents * normPerPerson * 15m / 30m;
            decimal secondHalfVolume = secondHalfResidents * normPerPerson * 16m / 30m;
            return firstHalfVolume + secondHalfVolume;
        }
    }

}
