import { startHubConnection } from "./useSignalRConnection";

beforeEach(() => jest.useFakeTimers());
afterEach(() => jest.useRealTimers());

test("retries initial failure, refreshes on reconnection, and stops retries after cleanup", async () => {
  const connection = {
    start: jest.fn().mockRejectedValueOnce(new Error("offline")).mockResolvedValue(undefined),
    stop: jest.fn().mockResolvedValue(undefined),
    onreconnected: jest.fn(), onclose: jest.fn(),
  };
  const refresh = jest.fn();
  const stop = startHubConnection(connection, refresh);
  await expect(connection.start.mock.results[0].value).rejects.toThrow("offline");
  expect(refresh).not.toHaveBeenCalled();
  jest.advanceTimersByTime(1000);
  await expect(connection.start.mock.results[1].value).resolves.toBeUndefined();
  expect(connection.start).toHaveBeenCalledTimes(2);
  expect(refresh).toHaveBeenCalledTimes(1);
  connection.onreconnected.mock.calls[0][0]();
  expect(refresh).toHaveBeenCalledTimes(2);
  connection.onclose.mock.calls[0][0]();
  stop();
  jest.advanceTimersByTime(60000);
  expect(connection.start).toHaveBeenCalledTimes(2);
  expect(connection.stop).toHaveBeenCalled();
});
