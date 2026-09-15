import moment from "moment";
import { DateFunctions } from "./dateFunctions";

test("monthly counts include single-digit dates and exclude other months and years", () => {
  const counts = DateFunctions.countPostsByDay([
    "2026-09-01T12:00:00", "2026-09-09T12:00:00", "2026-09-15T12:00:00",
    "2026-08-15T12:00:00", "2025-09-15T12:00:00", "invalid",
  ], moment("2026-09-15"));
  expect(counts).toHaveLength(30);
  expect(counts[0]).toBe(1);
  expect(counts[8]).toBe(1);
  expect(counts[14]).toBe(1);
  expect(counts.reduce((sum, value) => sum + value, 0)).toBe(3);
});
