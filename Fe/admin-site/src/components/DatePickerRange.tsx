"use client";

import * as React from "react";
import { Clock } from "lucide-react";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";

export function TimePickerWithRange({
  className,
  onTimeChange,
}: React.HTMLAttributes<HTMLDivElement> & {
  onTimeChange?: (timeRange: { from: string; to: string }) => void;
}) {
  const [timeRange, setTimeRange] = React.useState<{
    from: string;
    to: string;
  }>({
    from: "09:00",
    to: "17:00",
  });

  const generateHourOptions = () => {
    const options = [];
    for (let hour = 0; hour < 24; hour++) {
      for (const minute of [0, 30]) {
        const formattedHour = hour.toString().padStart(2, "0");
        const formattedMinute = minute.toString().padStart(2, "0");
        options.push(`${formattedHour}:${formattedMinute}`);
      }
    }
    return options;
  };

  const hourOptions = generateHourOptions();

  const handleChange = (field: "from" | "to", value: string) => {
    const updatedTimeRange = {
      ...timeRange,
      [field]: value,
    };
    setTimeRange(updatedTimeRange);

    if (onTimeChange) {
      onTimeChange(updatedTimeRange);
    }
  };

  return (
    <div className={cn("grid gap-2", className)}>
      <Popover>
        <PopoverTrigger asChild>
          <Button
            id="time"
            variant={"outline"}
            className={cn(
              "w-[300px] justify-start text-left font-normal",
              "flex items-center gap-2"
            )}
          >
            <Clock className="h-4 w-4" />
            <span>
              {timeRange.from} - {timeRange.to}
            </span>
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-auto p-4" align="start">
          <div className="flex items-center gap-4">
            <div>
              <label
                htmlFor="from-time"
                className="block text-sm font-medium text-gray-700"
              >
                From
              </label>
              <select
                id="from-time"
                value={timeRange.from}
                onChange={(e) => handleChange("from", e.target.value)}
                className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
              >
                {hourOptions.map((time) => (
                  <option key={time} value={time}>
                    {time}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label
                htmlFor="to-time"
                className="block text-sm font-medium text-gray-700"
              >
                To
              </label>
              <select
                id="to-time"
                value={timeRange.to}
                onChange={(e) => handleChange("to", e.target.value)}
                className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
              >
                {hourOptions.map((time) => (
                  <option key={time} value={time}>
                    {time}
                  </option>
                ))}
              </select>
            </div>
          </div>
        </PopoverContent>
      </Popover>
    </div>
  );
}
