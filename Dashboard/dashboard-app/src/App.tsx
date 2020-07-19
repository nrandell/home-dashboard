import React, { useEffect } from "react";
import "./App.css";
import { DataSource } from "./DataSource";

async function requestWakeLock() {
  try {
    const wakeLock = await navigator.wakeLock.request("screen");
    wakeLock.addEventListener("release", () =>
      console.log("Screen wake lock was released")
    );
    console.log("Screen wake lock is active");
    return wakeLock;
  } catch (err) {
    console.error(`Failed to activate wake lock: ${err}`);
  }
}
function App() {
  useEffect(() => {
    if ("wakeLock" in navigator) {
      let wakeLockSentinel: WakeLockSentinel | undefined;

      requestWakeLock().then((wl) => (wakeLockSentinel = wl));

      return () => {
        wakeLockSentinel?.release();
      };
    }
  }, []);

  return (
    <div className="App">
      <DataSource />
    </div>
  );
}

export default App;
