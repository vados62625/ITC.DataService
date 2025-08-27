import { Bar, BarConfig } from '@ant-design/plots';
import React from "react";
import { useParams } from 'react-router-dom'
import { DefectName, DefectNameType, DefectType, Engine } from '../../../types';
import { EngineApi } from '../../../apiRTK';
import { Loader } from '@consta/uikit/Loader';
import { EngineDto } from 'src/api';

type Card = {
  name: DefectNameType,
  probability: string
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
      const lastItem = defect.history?.at(-1)

      const card: Card = {
        name: DefectName[getLocalType(defect.type)],
        probability: lastItem?.probability ? (lastItem.probability * 100).toFixed(3) : '0'
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

  const config: BarConfig = {
    data: cardData,
    xField: 'name',
    yField: 'probability',
    style: {
      maxWidth: 25,
      fill: (item: Card) => {
        const probability = Number(item.probability.split('.')[0])

        if (probability > 50) {
          return 'rgba(235, 87, 87, 1)';
        }
        if (probability > 25) {
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







