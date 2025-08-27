import { Bar } from '@ant-design/plots';
import React from "react";
import { useParams } from 'react-router-dom'
import { DefectName, DefectNameType, DefectType, Engine } from '../../../types';
import { EngineApi } from '../../../apiRTK';
import { Loader } from '@consta/uikit/Loader';
import { EngineDto } from 'src/api';

type Card = {
  name: DefectNameType,
  value: number
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

const adapter = (engine: EngineDto | undefined): Card[] => {
  if (!engine || (engine.defects ?? []).length === 0) return []

  return (engine.defects ?? []).reduce((acc: Card[], defect) => {

    if ((defect.history ?? []).length > 0) {
      const card: Card = {
        name: DefectName[getLocalType(defect.type)],
        value: defect.history && defect.history?.length !== 0 ? (defect.history[0].probability ?? 0)*100 : 0
      }

      acc.push(card)
    }

    return acc
  }, [])
}

export const ReportCard = () => {
  const { id = '' } = useParams()
  const { data, isLoading } = EngineApi.useGetByIdQuery(id, { skip: !id })

  const cardData: Card[] = adapter(data)

  const config = {
    data: cardData,
    xField: 'name',
    yField: 'value',
    style: {
      maxWidth: 25,
      fill: (item: Card) => {
        if (item.value > 50) {
          return 'rgba(235, 87, 87, 1)';
        }
        if (item.value > 25) {
          return 'rgba(243, 139, 0, 1)';
        }
        return 'rgba(86, 185, 242, 1)';
      },
    },
    markBackground: {
      style: {
        fill: '#eee',
      },
    },
    scale: {
      y: {
        domain: [0, 100],
      },
    },
    label: {
      formatter: (value: string) => {
        return `${value}% `;
      },
      style: {
        fill: '#ffffffff',
        fillOpacity: 1,
        fontSize: 14,
      },
    },
    tooltip: false
  };

  if (isLoading) {
    return <Loader style={{ height: "100%", width: "100%" }} />
  }

  return <Bar {...config} />;
}







