import React, { useState } from "react";
import { DefectName, DefectType } from "../../../types";
import { Line } from "@ant-design/charts";
import { Select } from "@consta/uikit/Select";

type LineData = {
    date: Date,
    value: number
    type: DefectType
}

export const History = () => {
    const [selectedType, setSelectedType] = useState<DefectType>('CAGE')

    const fulllineData: LineData[] = [
        { date: new Date("2024-01-15T10:30:00Z"), value: 15, type: "CAGE" },
        { date: new Date("2024-01-15T10:30:00Z"), value: 35, type: "INNER_RING" },
        { date: new Date("2024-01-15T10:30:00Z"), value: 38, type: "MISALIGNMENT" },
        { date: new Date("2024-01-15T10:30:00Z"), value: 45, type: "OUTER_RING" },
        { date: new Date("2024-01-15T10:30:00Z"), value: 85, type: "ROLLING_ELEMENTS" },
        { date: new Date("2024-01-15T10:30:00Z"), value: 95, type: "UNBALANCE" },

        { date: new Date("2024-01-22T14:45:00Z"), value: 18, type: "CAGE" },
        { date: new Date("2024-01-22T14:45:00Z"), value: 38, type: "INNER_RING" },
        { date: new Date("2024-01-22T14:45:00Z"), value: 40, type: "MISALIGNMENT" },
        { date: new Date("2024-01-22T14:45:00Z"), value: 48, type: "OUTER_RING" },
        { date: new Date("2024-01-22T14:45:00Z"), value: 88, type: "ROLLING_ELEMENTS" },
        { date: new Date("2024-01-22T14:45:00Z"), value: 98, type: "UNBALANCE" },

        { date: new Date("2024-01-29T09:15:00Z"), value: 19, type: "CAGE" },
        { date: new Date("2024-01-29T09:15:00Z"), value: 40, type: "INNER_RING" },
        { date: new Date("2024-01-29T09:15:00Z"), value: 41, type: "MISALIGNMENT" },
        { date: new Date("2024-01-29T09:15:00Z"), value: 50, type: "OUTER_RING" },
        { date: new Date("2024-01-29T09:15:00Z"), value: 90, type: "ROLLING_ELEMENTS" },
        { date: new Date("2024-01-29T09:15:00Z"), value: 100, type: "UNBALANCE" },

        { date: new Date("2024-01-10T11:20:00Z"), value: 12, type: "CAGE" },
        { date: new Date("2024-01-10T11:20:00Z"), value: 32, type: "INNER_RING" },
        { date: new Date("2024-01-10T11:20:00Z"), value: 35, type: "MISALIGNMENT" },
        { date: new Date("2024-01-10T11:20:00Z"), value: 42, type: "OUTER_RING" },
        { date: new Date("2024-01-10T11:20:00Z"), value: 82, type: "ROLLING_ELEMENTS" },
        { date: new Date("2024-01-10T11:20:00Z"), value: 92, type: "UNBALANCE" },

        { date: new Date("2024-01-05T16:00:00Z"), value: 10, type: "CAGE" },
        { date: new Date("2024-01-05T16:00:00Z"), value: 30, type: "INNER_RING" },
        { date: new Date("2024-01-05T16:00:00Z"), value: 33, type: "MISALIGNMENT" },
        { date: new Date("2024-01-05T16:00:00Z"), value: 40, type: "OUTER_RING" },
        { date: new Date("2024-01-05T16:00:00Z"), value: 80, type: "ROLLING_ELEMENTS" },
        { date: new Date("2024-01-05T16:00:00Z"), value: 90, type: "UNBALANCE" }
    ];

    const defectTypes: DefectType[] = ["CAGE", "INNER_RING", "MISALIGNMENT", "OUTER_RING", "ROLLING_ELEMENTS", "UNBALANCE"];

    const dataByType: Record<DefectType, LineData[]> = {} as Record<DefectType, LineData[]>;

    defectTypes.forEach(type => {
        dataByType[type] = fulllineData.filter(item => item.type === type);
    });

    const getLineData = (type: DefectType) => {
        switch (type) {
            case "CAGE":
                return dataByType.CAGE
            case "INNER_RING":
                return dataByType.INNER_RING
            case "MISALIGNMENT":
                return dataByType.MISALIGNMENT
            case "OUTER_RING":
                return dataByType.OUTER_RING
            case "ROLLING_ELEMENTS":
                return dataByType.ROLLING_ELEMENTS
            case "UNBALANCE":
                return dataByType.UNBALANCE

            default:
                return [];
        }
    }

    const getItemName = (item: DefectType) => {
        return DefectName[item]
    }

    return (
        <>
            <Select
                placeholder="Выберите дефект"
                className="m-b-4 p-h-4"
                size="s"
                value={selectedType}
                onChange={({ value }) => {
                    setSelectedType(value ?? 'CAGE')
                }}
                items={defectTypes}
                getItemLabel={getItemName}
                getItemKey={getItemName}
            />
            <Line data={getLineData(selectedType)} xField="date" yField="value" />
        </>
    )
}