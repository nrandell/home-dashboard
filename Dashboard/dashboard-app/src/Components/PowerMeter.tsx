import React from "react";
import classNames from "classnames";

interface Props {
  power: number;
  className: string;
}

export const PowerMeter: React.FC<Props> = (props) => {
  const { power, className } = props;

  return (
    <div className={classNames("power-meter-size", className)}>
      <span>{power}</span>
    </div>
  );
};
