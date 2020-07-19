import { Data, HildebrandState } from "../Models";

export async function retrieveHistory() {
  const response = await fetch("/history");
  const obj = (await response.json()) as Data[];

  const results = obj.map((o) => JSON.parse(o.json) as HildebrandState);
  return results;
}
