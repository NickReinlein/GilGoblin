const itemPath    = `item/`;
const recipePath  = `recipe/`;
const pricePath   = `price/`;
const craftPath   = `craft/`;
const worldPath   = `world/`;

const ORIGIN = window.location.origin;  

const API_PREFIX = "/api/";

function buildUrl(tabName: string, id: number | null, world: number | null) {
  let suffix: string;
  switch (tabName) {
    case "Items":
      suffix = `${itemPath}${id}`;
      break;
    case "Recipes":
      suffix = `${recipePath}${id}`;
      break;
    case "Prices":
      suffix = `${pricePath}${world}/${id}/false`;
      break;
    case "Profits":
      suffix = `${craftPath}${world}`;
      break;
    case "World":
      suffix = `${worldPath}`;
      break;
    default:
      suffix = "";
  }
  return `${ORIGIN}${API_PREFIX}${suffix}`;
}

export const fetchData = async (
  tabName: string,
  id: number | null,
  world: number | null
): Promise<any> => {
  try {
    const url = buildUrl(tabName, id, world);
    console.log(`Fetching from URL: ${url}`);
    const response = await fetch(url, {});
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }
    return await response.json();
  } catch (error) {
    console.error("Error fetching data:", error);
    throw error;
  }
};

export type DataFetcherType = {
  fetchData: (tabName: string, id: number | null, world: number | null) => Promise<any>;
};

const DataFetcher: DataFetcherType = {
  fetchData,
};

export default DataFetcher;