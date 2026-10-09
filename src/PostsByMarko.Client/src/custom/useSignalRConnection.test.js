import { startHubConnection } from "./useSignalRConnection";
import { vi } from "vitest";

beforeEach(() => vi.useFakeTimers());
afterEach(() => vi.useRealTimers());

test("retries initial failure, refreshes on reconnection, and stops retries after cleanup", async () => {
  // Arrange
  const connection = {
    start: vi.fn().mockRejectedValueOnce(new Error("offline")).mockResolvedValue(undefined),
    stop: vi.fn().mockResolvedValue(undefined),
    onreconnected: vi.fn(),
    onclose: vi.fn(),
  };
  const refresh = vi.fn();
  // Act
  const stop = startHubConnection(connection, refresh);

  // Assert
  await expect(connection.start.mock.results[0].value).rejects.toThrow("offline");
  expect(refresh).not.toHaveBeenCalled();
  // Act
  vi.advanceTimersByTime(1000);

  // Assert
  await expect(connection.start.mock.results[1].value).resolves.toBeUndefined();
  expect(connection.start).toHaveBeenCalledTimes(2);
  expect(refresh).toHaveBeenCalledTimes(1);
  // Act
  connection.onreconnected.mock.calls[0][0]();

  // Assert
  expect(refresh).toHaveBeenCalledTimes(2);
  // Act
  connection.onclose.mock.calls[0][0]();
  stop();
  vi.advanceTimersByTime(60000);
  // Assert
  expect(connection.start).toHaveBeenCalledTimes(2);
  expect(connection.stop).toHaveBeenCalled();
});
