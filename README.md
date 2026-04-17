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

### Drill-Down View

### Network RAG Overview

### 🤖 Seasonal & ML Insights
