import React, { useEffect } from "react";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";
import { Line } from "react-chartjs-2";
import { callback } from "chart.js/helpers";

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
);

// Set global default colors
ChartJS.defaults.color = "#f9f5eb"; // Font color

export const options = {
  responsive: true,
  scales: {
    y: {
      ticks: {
        stepSize: 1,
      },
      grid: {
        color: "rgb(82, 86, 82)",
      },
    },
    x: {
      grid: {
        color: "rgb(82, 86, 82)",
      },
    },
  },
  plugins: {
    legend: {
      position: "top",
      labels: {
        padding: 20,
        font: {
          size: 16,
          weight: "bold",
        },
      },
    },

    tooltip: {
      enabled: true,
      callbacks: {
        label: function (context) {
          return ` ${context.parsed.y} users registered`;
        },

        title: function (context) {
          return context[0].label;
        },
      },
    },
  },
};

const LineChart = ({ title, labels, data }) => {
  const chartData = {
    labels: labels,
    datasets: [
      {
        label: title,
        data: data,
        borderColor: "rgb(96, 126, 170)",
        borderWidth: 2,
        tension: 0.3,
      },
    ],
  };

  useEffect(() => {}, [data.length]);

  return <Line options={options} data={chartData} />;
};

export default LineChart;
