import React from "react";
import { FILTERS } from "../constants/filters";

const PostFilters = ({ onFilterToggle }) => {
  return (
    <div className="dashboard__filters">
      {FILTERS.postFilters.map((filter) => (
        <span className="filter" key={filter.name}>
          {filter.text}
          <input
            type={filter.type}
            name={filter.name}
            id={filter.name}
            onChange={(e) => onFilterToggle(e.target.checked, e.target.name)}
          />
        </span>
      ))}
    </div>
  );
};

export default PostFilters;
