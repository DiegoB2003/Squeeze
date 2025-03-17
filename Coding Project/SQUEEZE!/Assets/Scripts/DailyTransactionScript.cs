using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using XCharts.Runtime;

namespace XCharts.Example
{
    [DisallowMultipleComponent]
    public class DailyTransactionScript : MonoBehaviour
    {
        private List<int> totals = ShopManager.Instance.totalMoneyForDay;
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
            chart.GetChartComponent<Title>().text = "Daily Transactions Report";
            chart.GetChartComponent<Title>().subText = "Each transaction for the day";

            //Configure Y-axis
            var yAxis = chart.GetChartComponent<YAxis>();
            yAxis.minMaxType = Axis.AxisMinMaxType.Default;
            yAxis.axisName.name = "Balance ($)";
            yAxis.axisName.show = true;

            //Configure X-axis
            var xAxis = chart.GetChartComponent<XAxis>();
            xAxis.axisName.name = "Transaction";
            xAxis.axisName.show = true;

            //Add and configure series
            serie = chart.AddSerie<Line>("Transaction - Balance");
            serie.symbol.show = true;
            serie.lineType = LineType.Normal;

            //Add data points
            for (int i = 0; i < totals.Count; i++)
            {
                chart.AddXAxisData((i+1).ToString());
                chart.AddData(0, totals[i]);
            }

            //Refresh the chart immediately
            chart.RefreshChart();
            
            yield return null;
        }
    }
}