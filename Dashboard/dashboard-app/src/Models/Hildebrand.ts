export interface HildebrandState {
  utcTimestamp: Date;
  hardwareVersion: string;
  siteId: string;
  rssi: number;
  lqi: number;
  wattHoursDelivered: number;
  currentWatts: number;
  todayWattHours: number;
  thisWeekWattHours: number;
  thisMonthWattHours: number;
}

export interface Data {
  topic: string;
  json: any;
}
