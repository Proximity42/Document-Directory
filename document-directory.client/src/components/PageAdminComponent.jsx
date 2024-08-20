import React, { Component } from 'react';
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Divider, Menu, Switch } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import ListUserComponent from './ListUserComponent';

function PageAdminComponent() {

    const menuItems = [
        {
            key: '1',
            icon: <UserOutlined />,
            label: 'Все пользователи',
        },
        {
            key: '2',
            label: 'Все группы',
        }
    ]

    return (
        <>
            <h1>Управление</h1>
            <div style={{ display: 'flex' }}>
                <div>
                    <Menu
                        style={{
                            width: 256,
                        }}
                        defaultSelectedKeys={['2']}
                        
                        items={menuItems}
                    />
                </div>
                <div>
                    <ListUserComponent />
                </div>
            </div>
            
        </>
    )
}
export default PageAdminComponent;