import React, { useState, useEffect } from "react";

import * as signalR from "@microsoft/signalr";
import { RawData } from "./RawData";
import { HildebrandSource } from "./Services";
import { HildebrandState } from "./Models";
import { PowerMeter } from "./Components";

interface Props {}

interface Data {
  topic: string;
  json: any;
}

export const DataSource: React.FC<Props> = (props) => {
  const [data, setData] = useState<Data>();

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("/data")
      .withAutomaticReconnect()
      .build();

    connection.on("Data", (topic: string, json: string) => {
      var jsonData = JSON.parse(json);
      const data = { topic, json: jsonData };
      setData(data);
    });
    connection
      .start()
      .then(() => console.log("started"))
      .catch(document.write);

    return () => {
      console.log("stopping");
      connection
        .stop()
        .then(() => console.log("stopped"))
        .catch(document.write);
    };
  }, []);

  if (data) {
    const state = data.json as HildebrandState;
    return (
      <div className="w-100 h-100 grid power-meter-grid">
        <RawData
          className="grid-area-info info-size"
          json={JSON.stringify(data, undefined, "  ")}
        />
        <PowerMeter
          className="grid-area-power flex w-100 justify-center"
          power={state.currentWatts}
        />
      </div>
    );
  } else {
    return (
      <div>
        <h1>Connecting...</h1>
      </div>
    );
  }
};
