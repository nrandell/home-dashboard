interface WakeLock {
  request(type: string): Promise<WakeLockSentinel>;
}

interface WakeLockSentinel {
  release(): Promise<void>;
  readonly onrelease: EventHandlerNonNull;
  readonly WakeLocktype: WakeLockType;
  addEventListener(
    event: string,
    callback: EventListenerOrEventListenerObject,
    options?: any
  );
  removeEventListener(
    event: string,
    callback: EventListenerOrEventListenerObject,
    options?: any
  );
}

type WakeLockType = "screen" | null;

interface Navigator {
  // Only available in a secure context.
  readonly wakeLock: WakeLock;
}
