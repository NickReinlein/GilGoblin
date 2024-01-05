const itemPath = `item/`;
const recipePath = `recipe/`;
const pricePath = `price/`;
const craftPath = `craft/`;
const getBaseUrl = `http://localhost:55448/`;
const fetchData = async (tabName: string, id: number, world: number) => {
    try {
        let suffix: string;
        switch (tabName) {
            case 'Items':
                suffix = `${itemPath}${id}`;
                break;
            case 'Recipes':
                suffix = `${recipePath}${id}`;
                break;
            case 'Prices':
                suffix = `${pricePath}${world}/${id}`;
                break;
            case 'Crafts':
                suffix = `${craftPath}${world}/${id}`;
                break;
            default:
                suffix = ``;
                break;
        }
        let url = `${getBaseUrl}${suffix}`;
        console.log(`Fetching from url ${url}`)
        const response = await fetch(url);
        return await response.json();
    } catch (error) {
        console.error('Error fetching data:', error);
    }
};

const DataFetcher = {
    fetchData,
};

export default DataFetcher;
