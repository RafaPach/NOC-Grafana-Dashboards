# NOC-Grafana-Dashboards
A modular observability platform built with .NET that centralises monitoring data from multiple sources into unified Grafana dashboards, enabling faster triage and improved operational visibility.

## Overview
Grafana NOC API is designed to aggregate and standardise monitoring data across multiple systems, providing a single source of truth for system health, availability, and performance.
It supports NOC operations by reducing tool fragmentation, improving visibility, and enabling quicker incident response through structured, real-time dashboards.

## Problem
In many NOC environments, monitoring data is distributed across multiple tools (e.g. SolarWinds, AlertSite, Google Analytics), which leads to:

- Lack of a unified system health view  
- Slower incident triage due to context switching  
- Inconsistent alerting and visibility across services  

This project addresses these challenges by consolidating observability data into a centralised platform.

## Architecture
- Built using **.NET (C#)** with a modular, plugin-based architecture  
- Each data source is implemented as an independent module:
  - AlertSite  
  - SolarWinds  
  - Google Analytics  
  - FTP Screen Scraping  

- Standardised metric structure for consistent ingestion into **Prometheus**  
- Data visualised using **Grafana dashboards**

- ## Tech Stack
- .NET (C#)  
- Prometheus  
- Grafana  
- REST APIs  
- Azure (Monitoring & Insights)  
- Python (Machine Learning)  


📊 Dashboards

### Top 10 Applications RAG Overview

<img width="1877" height="855" alt="image" src="https://github.com/user-attachments/assets/842b3dfe-f820-4330-a2b4-a79cb86e2309" />

### Drill-Down View

<img width="1875" height="760" alt="image" src="https://github.com/user-attachments/assets/cf3ea5e2-e287-447d-a2b8-c8b2ac5c4b82" />

<img width="1880" height="562" alt="image" src="https://github.com/user-attachments/assets/8bdf4ada-8e56-42de-b764-669eaf020eb4" />

### Network RAG Overview

<img width="1891" height="865" alt="image" src="https://github.com/user-attachments/assets/997d1295-6992-471d-b99d-23ad8a305d98" />

### Seasonal & ML Insights

<img width="1884" height="838" alt="image" src="https://github.com/user-attachments/assets/687ad2c8-48da-4b61-97b4-595a442d3773" />


## Future Improvements

-   Improved alert correlation and automation  
-   Enhanced dashboard filtering and costumisation
-   Additional integrations for cloud monitoring
