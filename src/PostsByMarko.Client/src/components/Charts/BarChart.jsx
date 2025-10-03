import React from "react";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";
import { Bar } from "react-chartjs-2";
import { DateFunctions } from "../../util/dateFunctions";

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
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
          var numberOfPosts = context.parsed.y;

          return ` ${numberOfPosts} ${
            numberOfPosts === 1 ? "post was created" : "posts were created"
          }`;
        },

        title: function (context) {
          return `${context[0].label} ${DateFunctions.getThisMonthAsText()}`;
        },
      },
    },
  },
};

const BarChart = ({ title, labels, data }) => {
  const chartData = {
    labels: labels,
    datasets: [
      {
        label: title,
        data: data,
        backgroundColor: "rgba(96, 126, 170, 0.5)",
        borderColor: "rgb(96, 126, 170)",
        borderWidth: 2,
      },
    ],
  };

  return <Bar options={options} data={chartData} />;
};

export default BarChart;
