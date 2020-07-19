import React, { useState, useEffect, useContext } from "react";
import { HildebrandState } from "../Models";
import { retrieveHistory } from "./";

interface Props {
  state: HildebrandState;
  className: string;
}

export const StateHistory: React.FC<Props> = (props) => {
  const { state, className } = props;
  const [stateHistory, setStateHistory] = useState<HildebrandState[]>([]);

  console.log("state", state);

  useEffect(() => {
    retrieveHistory().then((newHistory) => {
      console.log("new history", newHistory);
      setStateHistory(newHistory);
    });
  }, []);

  useEffect(() => {
    setStateHistory((old) => [...old, state]);
  }, [state]);

  const watts = stateHistory.map((s) => s.currentWatts);

  return (
    <div className={className}>
      <pre>{JSON.stringify(watts)}</pre>;
    </div>
  );
};
