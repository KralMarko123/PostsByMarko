import React, { useEffect } from "react";
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
import { HelperFunctions } from "../../util/helperFunctions";

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

export const options = {
  responsive: true,
  plugins: {
    legend: {
      position: "top",
    },
  },
};

const labels = [
  ...Array(HelperFunctions.getCurrentMonthDayNumber()).keys(),
].map((i) => i + 1);

const BarChart = ({ posts }) => {
  const data = labels.map(
    (l) =>
      posts.filter(
        (p) =>
          HelperFunctions.getDayOfMonthFromDate(p.createdDate) === l.toString()
      ).length
  );

  const chartData = {
    labels: labels,
    datasets: [
      {
        label: "Number of posts this month",
        data: data,
        backgroundColor: "rgb(96, 126, 170)",
      },
    ],
  };

  useEffect(() => {}, [posts.length]);

  return <Bar options={options} data={chartData} />;
};

export default BarChart;
