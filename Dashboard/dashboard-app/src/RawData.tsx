import React from "react";

interface Props {
  json: string;
}

export const RawData: React.FC<Props> = (props) => {
  const { json } = props;

  return (
    <div>
      <pre>{json}</pre>
    </div>
  );
};
