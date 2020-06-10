import React, { useState, useEffect } from "react";

import * as signalR from "@microsoft/signalr";
import { RawData } from "./RawData";

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
    return <RawData json={JSON.stringify(data, undefined, "  ")} />;
  } else {
    return (
      <div>
        <h1>Connecting...</h1>
      </div>
    );
  }
};
