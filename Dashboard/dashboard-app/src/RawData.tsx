import React from "react";

interface Props {
  className?: string;
  json: string;
}

export const RawData: React.FC<Props> = (props) => {
  const { json, className } = props;

  return (
    <div className={className}>
      <pre>{json}</pre>
    </div>
  );
};
