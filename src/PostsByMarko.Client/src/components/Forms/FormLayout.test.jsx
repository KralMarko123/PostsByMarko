import React, { act } from "react";
import { createRoot } from "react-dom/client";
import { vi } from "vitest";
import { FormLayout } from "./FormLayout";
import { Button } from "../Helper/Button/Button";

global.IS_REACT_ACT_ENVIRONMENT = true;

test("form submissions prevent navigation and reject duplicates while pending", async () => {
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
  await act(async () =>
    root.render(
      <FormLayout title="Sign In" description="Welcome" onSubmit={submit}>
        <Button type="submit" text="Sign In" />
      </FormLayout>,
    ),
  );
  const form = container.querySelector("form");
  const submission = new Event("submit", { bubbles: true, cancelable: true });

  try {
    // Act
    await act(async () => {
      form.dispatchEvent(submission);
      form.dispatchEvent(new Event("submit", { bubbles: true, cancelable: true }));
    });

    // Assert
    expect(submission.defaultPrevented).toBe(true);
    expect(submit).toHaveBeenCalledTimes(1);
    expect(container.querySelector("button").type).toBe("submit");

    // Act
    await act(async () => finish());
    await act(async () =>
      form.dispatchEvent(new Event("submit", { bubbles: true, cancelable: true })),
    );

    // Assert
    expect(submit).toHaveBeenCalledTimes(2);
    await act(async () => finish());
  } finally {
    // Cleanup
    await act(async () => root.unmount());
  }
});
