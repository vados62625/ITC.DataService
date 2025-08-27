import { Defect } from "../defect"

export type EngineStatus = 'new' |'pending' | 'failed' | 'succses'

export type Engine = {
    id: string,
    isLastAnalyseHasDefect: boolean,
    status: EngineStatus,
    defects: Defect[],
    lastAnalyseDate: Date,
}

