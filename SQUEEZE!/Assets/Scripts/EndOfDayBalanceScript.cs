using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using XCharts.Runtime;

namespace XCharts.Example
{
    [DisallowMultipleComponent]
    public class EndOfDayBalanceScript : MonoBehaviour
    {
        private List<int> balances = ShopManager.Instance.EODBalance;
        private LineChart chart;
        private Serie serie;

        private void OnEnable()
        {
            StartCoroutine(InitializeChart());
        }

        IEnumerator InitializeChart()
        {
            chart = gameObject.GetComponent<LineChart>();
            if (chart == null)
            {
                chart = gameObject.AddComponent<LineChart>();
                chart.Init();
            }

            //Clear previous data
            chart.RemoveData();
            
            //Configure chart titles
            chart.GetChartComponent<Title>().text = "EOD Balance Report";
            chart.GetChartComponent<Title>().subText = "Balance at the end of each day";

            //Configure Y-axis
            var yAxis = chart.GetChartComponent<YAxis>();
            yAxis.minMaxType = Axis.AxisMinMaxType.Default;
            yAxis.axisName.name = "Balance ($)";
            yAxis.axisName.show = true;

            //Configure X-axis
            var xAxis = chart.GetChartComponent<XAxis>();
            xAxis.axisName.name = "Day";
            xAxis.axisName.show = true;

            //Add and configure series
            serie = chart.AddSerie<Line>("Day - Balance");
            serie.symbol.show = true;
            serie.lineType = LineType.Normal;

            //Add data points
            for (int i = 0; i < balances.Count; i++)
            {
                chart.AddXAxisData((i+1).ToString());
                chart.AddData(0, balances[i]);
            }

            //Refresh the chart immediately
            chart.RefreshChart();
            
            yield return null;
        }
    }
}