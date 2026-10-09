import React, { act } from "react";
import { createRoot } from "react-dom/client";
import { Button } from "./Button";
import { vi } from "vitest";

global.IS_REACT_ACT_ENVIRONMENT = true;

test("a pending submission cannot be repeated", async () => {
  // Arrange
  const container = document.createElement("div");
  const root = createRoot(container);
  let finish;
  const submit = vi.fn(
    () =>
      new Promise((resolve) => {
        finish = resolve;
      }),
  );
  await act(async () => root.render(<Button text="Save" onButtonClick={submit} />));
  const button = container.querySelector("button");
  // Act
  await act(async () => {
    button.click();
    button.click();
  });

  // Assert
  expect(submit).toHaveBeenCalledTimes(1);
  expect(button.disabled).toBe(true);
  // Act
  await act(async () => finish());

  // Assert
  expect(button.disabled).toBe(false);
  // Cleanup
  await act(async () => root.unmount());
});
