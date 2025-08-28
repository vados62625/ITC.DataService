import { RegistryRow, TableFilterType } from 'src/types'
import { apiSlice, Clients, EngineDto } from '../../api'
import { FetchBaseQueryError } from '@reduxjs/toolkit/query'
import { addNotification } from '../../store/notification'

const getLocalStatus = (statusNumber: number | undefined) => {
    switch (statusNumber) {
        case 0:
            return 'new'
        case 1:
            return 'pending'
        case 2:
            return 'failed'
        case 3:
            return 'succses'
        default:
            return 'new'
    }
}

const adapter = (engine: EngineDto): RegistryRow => ({
    id: engine.id ?? '',
    name: engine.name ?? '',
    status: getLocalStatus(engine.status),
    isLastAnalyseHasDefect: !!engine.isLastAnalyseHasDefect,
    isLastAnalyseHasCriticalDefect: !!engine.isLastAnalyseHasCriticalDefect,
    lastAnalazeDate: String(engine.lastAnalyseDate),
    recommendedMaintenanceDate: engine.recommendedMaintenanceDate
})

export const EngineApi = apiSlice.injectEndpoints({
    endpoints: build => ({
        getAllByFilter: build.query<RegistryRow[], TableFilterType>({
            queryFn: async (filter, { dispatch }) => {
                const getFilter = (): '0' | "1" => {
                    if (filter === 'LIVE') {
                        return '0'
                    }
                    return '1'
                }

                try {
                    //   const data = await Clients.EnginesClient.enginesGet(filter)
                    // id, engineType, name, engineStatus, page, itemsCount
                    const data = await Clients.EnginesClient.enginesGet(undefined, getFilter(), undefined, undefined, 1, 1)
                    // const data: RegistryRow[] = [
                    //     {
                    //         id: '1',
                    //         name: 'Асинхронный двигатель АИР80В4',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '2',
                    //         name: 'Электродвигатель 5АМ112МВ6',
                    //         isHasDefect: true,
                    //         status: 'new',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '3',
                    //         name: 'Двигатель 4АМ90L2',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '4',
                    //         name: 'Мотор АИР71А2',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '5',
                    //         name: 'Электродвигатель ВАО72-4',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '6',
                    //         name: 'АИР100S4',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '7',
                    //         name: 'Двигатель 5АМ132S8',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '8',
                    //         name: 'Мотор АИР56В6',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '9',
                    //         name: 'Электродвигатель 4АМ80А4',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '10',
                    //         name: 'АИР63В2',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '11',
                    //         name: 'Двигатель 5АМ160М4',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '12',
                    //         name: 'Мотор ВАО82-6',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '13',
                    //         name: 'Электродвигатель АИР112МВ8',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '14',
                    //         name: 'АИР75С4',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '15',
                    //         name: 'Двигатель 4АМ132М2',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '16',
                    //         name: 'Мотор 5АМ90L6',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '17',
                    //         name: 'Электродвигатель АИР80А4',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '18',
                    //         name: 'ВАО63-2',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '19',
                    //         name: 'Двигатель 4АМ112МА6',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '20',
                    //         name: 'Мотор АИР71В4',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '21',
                    //         name: 'Электродвигатель 5АМ100L8',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '22',
                    //         name: 'АИР56А2',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '23',
                    //         name: 'Двигатель ВАО72-6',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '24',
                    //         name: 'Мотор 4АМ160S4',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '25',
                    //         name: 'Электродвигатель АИР132МА2',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '26',
                    //         name: 'АИР80В6',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '27',
                    //         name: 'Двигатель 5АМ112S4',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '28',
                    //         name: 'Мотор ВАО82-4',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '29',
                    //         name: 'Электродвигатель АИР90L8',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '30',
                    //         name: '4АМ75В2',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '31',
                    //         name: 'Двигатель АИР63С4',
                    //         isHasDefect: true,
                    //         status: 'failed',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '32',
                    //         name: 'Мотор 5АМ132М6',
                    //         isHasDefect: true,
                    //         status: 'pending',
                    //         lastAnalazeDate: '23.08.2025'
                    //     },
                    //     {
                    //         id: '33',
                    //         name: 'Электродвигатель ВАО71-8',
                    //         isHasDefect: false,
                    //         status: 'succses',
                    //         lastAnalazeDate: '23.08.2025'
                    //     }
                    // ]

                    return {
                        data: data.items?.map(adapter) ?? [],
                    }
                } catch (error) {
                    const fetchError: FetchBaseQueryError = {
                        status: 'CUSTOM_ERROR',
                        error: error instanceof Error ? error.message : 'Unknown error'
                    };
                    dispatch(
                        addNotification({
                            status: 'alert',
                            message: 'Ошибка загрузки данных реестра двигателей',
                        })
                    )

                    return {
                        error: fetchError,
                    }
                }
            },
            providesTags: (result) =>
                result
                    ? [
                        ...result.map(({ id }) => ({ type: 'Engine' as const, id })),
                        { type: 'Engine', id: 'LIST' },
                    ]
                    : [{ type: 'Engine', id: 'LIST' }],
        }),

        getById: build.query<EngineDto | undefined, string>({
            queryFn: async (id, { dispatch }) => {
                try {
                    // id, engineType, name, engineStatus, page, itemsCount
                    const data = await Clients.EnginesClient.enginesGet(id, undefined, undefined, undefined, 1, 1)
                    // const data: Engine = {
                    //     id: 'engineId',
                    //     isLastAnalyseHasDefect: false,
                    //     status: 'failed',
                    //     defects: [
                    //         {
                    //             engineId: 'engineId',
                    //             history: [{
                    //                 defectId: 'defectId1',
                    //                 date: new Date(),
                    //                 probability: 0.19
                    //             }],
                    //             type: 'CAGE',
                    //         }
                    //     ],
                    //     lastAnalyseDate: new Date()
                    // }

                    return { data: data.items ? data.items[0] : undefined }
                } catch (error) {
                    const fetchError: FetchBaseQueryError = {
                        status: 'CUSTOM_ERROR',
                        error: error instanceof Error ? error.message : 'Unknown error'
                    };
                    dispatch(
                        addNotification({
                            status: 'alert',
                            message: 'Ошибка загрузки данных двигателя',
                        })
                    )

                    return {
                        error: fetchError,
                    }
                }
            },
            providesTags: (result, error, id) => [
                { type: 'Engine', id },
                { type: 'Engine', id: 'LIST' }, // Также инвалидируем список
            ],
        }),

        add: build.mutation<EngineDto, { name: string | null, file: File | null }>({
            queryFn: async ({ file, name }, { dispatch }) => {

                const fileToUpload = {
                    fileName: file?.name ?? 'fileName.csv',
                    data: file,
                }

                try {
                    const data = await Clients.EnginesClient.enginesPost(name ?? "", fileToUpload)
                    dispatch(
                        addNotification({
                            status: 'success',
                            message: `Данные двигателя ${name} успешно загружены`,
                        })
                    )

                    return {
                        data,
                    }
                } catch (error) {
                    const fetchError: FetchBaseQueryError = {
                        status: 'CUSTOM_ERROR',
                        error: error instanceof Error ? error.message : 'Unknown error'
                    };
                    console.log('fetchError', fetchError);

                    return {
                        error: fetchError,
                    }
                }
            },
            invalidatesTags: [{ type: 'Engine', id: 'LIST' }],
        }),

        delete: build.mutation<void, string>({
            queryFn: async (id, { dispatch }) => {
                try {
                    const data = await Clients.EnginesClient.enginesDelete(id)
                    dispatch(
                        addNotification({
                            status: 'success',
                            message: `Данные двигателя успешно удалены`,
                        })
                    )   
                    return {
                        data,
                    }
                } catch (error) {
                    const fetchError: FetchBaseQueryError = {
                        status: 'CUSTOM_ERROR',
                        error: error instanceof Error ? error.message : 'Unknown error'
                    };
                    dispatch(
                        addNotification({
                            status: 'alert',
                            message: `Ошибка при удалении данных двигателя`,
                        })
                    )
                    return {
                        error: fetchError,
                    }
                }
            },
            invalidatesTags: (result, error, id) => [
                { type: 'Engine', id },
                { type: 'Engine', id: 'LIST' },
            ],
        }),
    }),
})
