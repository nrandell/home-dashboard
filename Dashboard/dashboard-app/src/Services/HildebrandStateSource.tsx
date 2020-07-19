import React from "react";
import { HildebrandState } from "../Models";

export const HildebrandStateContext = React.createContext<HildebrandState | null>(
  null
);

interface Props {
  state: HildebrandState;
}

export const HildebrandSource: React.FC<Props> = (props) => {
  const { state, children } = props;

  return (
    <HildebrandStateContext.Provider value={state}>
      {children}
    </HildebrandStateContext.Provider>
  );
};
