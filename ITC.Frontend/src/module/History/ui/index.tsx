import React, { useState } from "react";
import { DefectName, DefectType } from "../../../types";
import { Line } from "@ant-design/charts";
import { Select } from "@consta/uikit/Select";
import { useParams } from "react-router-dom";
import { EngineApi } from "../../../apiRTK";
import { Loader } from "@consta/uikit/Loader";
import { EngineDto } from "../../../api";
import { getRuDate } from "../../../utils";

type LineData = {
    date: Date | undefined,
    probability: string
    type: DefectType | undefined
}

const config: Intl.DateTimeFormatOptions = {
    day: 'numeric',
    month: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
}

const getLocalType = (typeNumber: number | undefined): DefectType => {
    switch (typeNumber) {
        case 0:
            return 'OUTER_RING'
        case 1:
            return 'INNER_RING'
        case 2:
            return 'ROLLING_ELEMENTS'
        case 3:
            return 'CAGE'
        case 4:
            return 'UNBALANCE'
        case 5:
            return 'MISALIGNMENT'
        default:
            return 'OUTER_RING'
    }
}

const adapter = (engine: EngineDto | undefined): LineData[] => {
    if (!engine || engine.defects?.length === 0) return []

    return (engine.defects ?? [])
        .reduce((acc: LineData[], defect) => {
            (defect.history ?? []).forEach(item => {
                const lineData: LineData = {
                    type: getLocalType(defect.type),
                    probability: (item.probability ?? 0).toFixed(3),
                    date: item.date
                }

                acc.push(lineData)
            })
            return acc
        }, [])
}

export const History = () => {
    const [selectedType, setSelectedType] = useState<DefectType>('CAGE')

    const { id = '' } = useParams()
    const { data, isLoading } = EngineApi.useGetByIdQuery(id, { skip: !id, refetchOnMountOrArgChange: true })

    const lineData = adapter(data)

    const defectTypes: DefectType[] = ["CAGE", "INNER_RING", "MISALIGNMENT", "OUTER_RING", "ROLLING_ELEMENTS", "UNBALANCE"];

    const dataByType: Record<DefectType, LineData[]> = {} as Record<DefectType, LineData[]>;

    defectTypes.forEach(type => {
        dataByType[type] = lineData.filter(item => item.type === type);
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

    if (isLoading) {
        return <Loader style={{ height: "100%", width: "100%" }} />
    }

    const config = {
        data: getLineData(selectedType),
        xField: 'date',
        yField: 'probability',
        seriesField: 'type',
        // scale: {
        //     color: {
        //         range: ['#2688FF', 'red'],
        //     },
        // },
        // style: {
        //     lineWidth: 2,
        //     lineDash: (items:string) => {
        //         const { type } = items[0];
        //         return type === 'Bor' ? [2, 4] : [0, 0];
        //     },
        // },
        // interaction: {
        //     tooltip: {
        //         render: (e, { title, items }) => {
        //             const list = items.filter((item) => item.value);
        //             return (
        //                 <div key={title}>
        //                     <h4>{title}</h4>
        //                     {list.map((item) => {
        //                         const { name, value, color } = item;
        //                         return (
        //                             <div>
        //                                 <div style={{ margin: 0, display: 'flex', justifyContent: 'space-between' }}>
        //                                     <div>
        //                                         <span
        //                                             style={{
        //                                                 display: 'inline-block',
        //                                                 width: 6,
        //                                                 height: 6,
        //                                                 borderRadius: '50%',
        //                                                 backgroundColor: color,
        //                                                 marginRight: 6,
        //                                             }}
        //                                         ></span>
        //                                         <span>{name}</span>
        //                                     </div>
        //                                     <b>{value}</b>
        //                                 </div>
        //                             </div>
        //                         );
        //                     })}
        //                 </div>
        //             );
        //         },
        //     },
        // },
        // legend: false,
    };

    return (
        <div className="m-b-2 m-l-2 m-r-2">
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
            <Line {...config} />
        </div>
    )
}