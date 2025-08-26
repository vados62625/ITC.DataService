import { Bar } from '@ant-design/plots';
import React from "react";
import { DefectName, DefectNameType } from '../../../types';

type Card = {
  name: DefectNameType,
  value: number
}

export const ReportCard = () => {
  const data: Card[] = [
    { name: DefectName['CAGE'], value: 19 },
    { name: DefectName['INNER_RING'], value: 40 },
    { name: DefectName['MISALIGNMENT'], value: 41 },
    { name: DefectName['OUTER_RING'], value: 50 },
    { name: DefectName['ROLLING_ELEMENTS'], value: 90 },
    { name: DefectName['UNBALANCE'], value: 100 },
  ];

  type Card = {
    name: DefectNameType;
    value: number;
  }

  const config = {
    data,
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
  return <Bar {...config} />;
}







