import axios from "axios";

// get VITE_BACKEND_URL from .env file
const VITE_BACKEND_URL = import.meta.env.VITE_BACKEND_URL;

export const api = axios.create({
    baseURL: VITE_BACKEND_URL,
    headers: {
        "Content-type": "application/json"
    }
});

export const uploadWithFile = async (
    data: { [key: string]: any; [fileKey]: File },
    method: "get" | "post" | "put" | "delete",
    path: string,
    fileKey: string
  ) => {
    const file = data[fileKey];
    const fileBase64 = await new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        resolve(reader.result as string);
      };
      reader.onerror = (error) => {
        reject(error); 
      };
    });
  

    data[fileKey] = fileBase64;
  
    console.log(data);
  
    // Send data to backend
    return api[method](path, data);
  };
  
export const useService = async (
    serviceName: string,
    data?: {}
  ) => { 
    const postData = {
      "service": serviceName,
      ...data
    }

    return api.post("/service", postData);
  };