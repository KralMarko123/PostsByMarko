import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
  TooltipItem,
  ChartData,
  ChartOptions,
} from "chart.js";
import { Bar } from "react-chartjs-2";
import { DateFunctions } from "../../util/dateFunctions";

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

// Set global default colors
ChartJS.defaults.color = "#f9f5eb"; // Font color

export const options: ChartOptions<"bar"> = {
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
          size: 18,
          weight: "bold",
        },
      },
    },

    tooltip: {
      enabled: true,
      callbacks: {
        label: function (context: TooltipItem<"bar">) {
          var numberOfPosts = context.parsed.y;

          return ` ${numberOfPosts} ${
            numberOfPosts === 1 ? "post was created" : "posts were created"
          }`;
        },

        title: function (context: TooltipItem<"bar">[]) {
          return `${context[0].label} ${DateFunctions.getThisMonthAsText()}`;
        },
      },
    },
  },
};

interface BarChartProps {
  title: string;
  labels: string[];
  data: number[];
}

export const BarChart = (props: BarChartProps) => {
  const chartData: ChartData<"bar"> = {
    labels: props.labels,
    datasets: [
      {
        label: props.title,
        data: props.data,
        backgroundColor: "rgba(96, 126, 170, 0.5)",
        borderColor: "rgb(96, 126, 170)",
        borderWidth: 2,
      },
    ],
  };

  return <Bar options={options} data={chartData} />;
};
