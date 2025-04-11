/**
 * A helper module for evaluating violation enums for this application
 * 
 * @param 
 * @returns
 * 
 */
export const evaluateClassification = (c) => {
    switch (c) {
        case 0:
            return 'Minor';
            break;
        case 1:
            return 'Major';
            break;
        case 2:
            return 'Minor Traffic';
            break;
        case 3:
            return 'Major Traffic';
            break;
    }
};
