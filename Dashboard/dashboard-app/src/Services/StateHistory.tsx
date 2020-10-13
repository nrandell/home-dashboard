import React, { useState, useEffect, useContext } from "react";
import { HildebrandState } from "../Models";
import { retrieveHistory } from "./";

const maxLength = 10;

interface Props {
  state: HildebrandState;
  className: string;
}

export const StateHistory: React.FC<Props> = (props) => {
  const { state, className } = props;
  const [stateHistory, setStateHistory] = useState<HildebrandState[]>([]);

  useEffect(() => {
    retrieveHistory().then((newHistory) => {
      setStateHistory(newHistory);
    });
  }, []);

  useEffect(() => {
    setStateHistory((old) => {
      const remaining =
        old.length > maxLength ? old.slice(old.length - maxLength) : old;
      return [...remaining, state];
    });
  }, [state]);

  const watts = stateHistory.map((s) => s.currentWatts);

  return (
    <div className={className}>
      <pre>{JSON.stringify(watts)}</pre>
    </div>
  );
};
